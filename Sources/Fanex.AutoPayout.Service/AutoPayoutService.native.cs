using AutoPayout.Sportsbook.Services;
using Fanex.Caching;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ONEbook.UM;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Transport;
using SharpRaven;

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
        StartDataBaseSetting();
        StartLogSetting();
    }

    private static void InitializeExtraComponent()
    {
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
    }

    private static void TaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved();
        e.Exception.Handle(x =>
        {
            LogService.Error(x.Message, x);
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
                Thread.Sleep(int.Parse(configuration.GetSection("ServiceInterval").Value));
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
            LogService.Error("Exception in main thread - Process:", exception);
        }
    }

    private void HandleGarbageCollector()
    {
        if (counter >= int.Parse(configuration.GetSection("GarbageCollectorInterval").Value))
        {
            counter = 0;
            GC.WaitForPendingFinalizers();
        }

        counter += int.Parse(configuration.GetSection("ServiceInterval").Value);
    }

    private void StartUmSetting()
    {
        var umConfig = new UMClientConfiguration().Initialize(configuration);

        UMClientManager
            .UseConfig(umConfig)
            .UseCacheService(new UMCacheService(new CacheService()))
            .UseRestClient(new UMRestClient(umConfig));
    }

    private void StartDataBaseSetting()
    {
        var connections = configuration.GetSection("ConnectionStrings")
            .GetChildren()
            .AsEnumerable()
            .ToDictionary(
                x => x.GetValue<string>("Name"),
                y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName"))
            );
        DbSettingProviderManager
            .StartNewSession()
            .Use(connections)
            .WithSqlServer()
            .UseDefaultDbSettingProvider(resourcePath: configuration.GetSection("StoredProcedurePath").Value)
            .Run();
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