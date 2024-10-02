using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ONEbook.UM.Services;

namespace Fanex.AutoPayout.Service.Services;

public class EarlyService(
    IConfiguration configuration,
    IMaintenanceService maintenanceService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(configuration.GetValue<int>("ServiceInterval")));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await HandleAutoAsync();
        }
    }

    private static async Task HandleAutoAsync()
    {
        try
        {
            //if (!maintenanceService.IsMaintenance)
            //{
            //    await autoMatchController.Process();
            //    await autoNotificationController.Process();
            //    HandleGarbageCollector();
            //}
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
}