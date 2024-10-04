using AutoPayout.Sportsbook.Services;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using LogService = Fanex.Logging.LogService;

namespace Fanex.AutoPayout.Service.Services;

public class AutoPayoutService(
    AutoPayoutController autoPayoutController,
    IConfiguration configuration,
    IMaintenanceService maintenanceService)
    : BackgroundService
{
    private int counter;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"Auto Payout Starting at {DateTime.Now}");

        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(configuration.GetValue<int>("ServiceInterval")));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await HandleAutoAsync();
        }
    }

    private async Task HandleAutoAsync()
    {
        try
        {
            if (!maintenanceService.IsMaintenance)
            {
                await autoPayoutController.ProcessAsync();
                HandleGarbageCollector();
            }
        }
        catch (Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .ToString();
            await LogService.ErrorAsync(logMessage);
        }
    }

    private void HandleGarbageCollector()
    {
        if (counter >= configuration.GetValue<int>("GarbageCollectorInterval"))
        {
            counter = 0;
            GC.WaitForPendingFinalizers();
        }

        counter += configuration.GetValue<int>("ServiceInterval");
    }
}