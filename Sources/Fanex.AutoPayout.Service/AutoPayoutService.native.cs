using AutoPayout.Sportsbook.Services;
using Fanex.Caching;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ONEbook.UM;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Transport;
using SharpRaven;
using LogService = Fanex.Logging.LogService;

namespace Fanex.AutoPayout.Service;

public class AutoPayoutService : IHostedService
{
    private readonly AutoPayoutController autoPayoutController;

    private readonly IMaintenanceService maintenanceService;

    private readonly IConfiguration configuration;

    private bool running;

    private int counter;

    public AutoPayoutService(
        AutoPayoutController autoPayoutController,
        IConfiguration configuration,
        IMaintenanceService maintenanceService)
    {
        this.autoPayoutController = autoPayoutController;
        this.configuration = configuration;
        this.maintenanceService = maintenanceService;
        InitializeExtraComponent();
        InitializeSetting();
    }

    private void InitializeSetting()
    {
        StartUmSetting();
        StartLogSetting();
    }

    private static void InitializeExtraComponent()
    {
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
    }

    private static void TaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        e.Exception.Handle(exception =>
        {
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .ToString();
            LogService.Error(logMessage);
            return true;
        });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        running = true;
        var autoRefundThread = new Thread(ThreadProcess);
        Console.WriteLine($"Auto Payout Starting at {DateTime.Now}");
        autoRefundThread.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        running = false;
        Console.WriteLine($"Auto Payout Stopping at {DateTime.Now}");
        return Task.CompletedTask;
    }

    private void ThreadProcess()
    {
        while (running)
        {
            if (!maintenanceService.IsMaintenance)
            {
                MainProcess();
                var value = configuration.GetSection("ServiceInterval").Value;
                if (value != null)
                {
                    Thread.Sleep(int.Parse(value));
                }
            }
        }
    }

    private void MainProcess()
    {
        try
        {
            autoPayoutController.ProcessAsync();
            HandleGarbageCollector();
        }
        catch (Exception exception)
        {
            Logging.LogService.Error("Fanex.AutoPayout.Service: ", exception);
        }
    }

    private void HandleGarbageCollector()
    {
        var value = configuration.GetSection("GarbageCollectorInterval").Value;
        if (value != null && counter >= int.Parse(value))
        {
            counter = 0;
            GC.WaitForPendingFinalizers();
        }

        var interval = configuration.GetSection("ServiceInterval").Value;
        if (interval != null)
        {
            counter += int.Parse(interval);
        }
    }

    private void StartUmSetting()
    {
        var umConfig = new UMClientConfiguration().Initialize(configuration);

        UMClientManager
            .UseConfig(umConfig)
            .UseCacheService(new UMCacheService(new CacheService()))
            .UseRestClient(new UMRestClient(umConfig));
    }

    private void StartLogSetting()
    {
        var logEngine = new List<ILogEngine>();

        var sentryEngineOptions = new SentryEngineOptions
        {
            Dsn = new Dsn(configuration.GetSection("SentryToken").Value)
        };

        logEngine.Add(new SentryLogging(sentryEngineOptions));

        LogManager.SetDefaultLogCategory(configuration.GetSection("SiteCategory").Value).Use(logEngine);
    }
}