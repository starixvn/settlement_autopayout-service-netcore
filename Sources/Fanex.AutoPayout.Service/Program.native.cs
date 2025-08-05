using AutoPayout.Core.Models;
using DirectPayout.Early;
using Fanex.AutoPayout.Service;
using Fanex.AutoPayout.Service.Services;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOneBookUmForService(builder.Configuration);

builder.Services.AddFanexLogSentry(builder.Configuration);

builder.Services.AddFanexSql(builder.Configuration);

builder.Services.AddConfigRedisCache(builder.Configuration);

builder.Services.AddAutoMapper();

builder.Services.Configure<AppSettingsKey>(builder.Configuration);

builder.Services.InitializeAutoPayoutServices(builder.Configuration);

builder.AddDirectPayoutEarlyModule();

builder.Services.AddHostedService<AutoPayoutService>();
builder.Services.AddHostedService<DirectPayoutEarlyService>();
builder.Services.AddHealthChecks();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], builder.Configuration["RabbitMQ:VirtualHost"], h =>
        {
            h.Username(builder.Configuration.GetValue("RabbitMQ:Username", "guest"));
            h.Password(builder.Configuration.GetValue("RabbitMQ:Password", "guest"));
        });

        cfg.UseMessageRetry(retryConfig =>
        {
            retryConfig.Interval(1, TimeSpan.FromSeconds(1));
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.Run(async context =>
{
    await context.Response.WriteAsync("OK");
});
app.MapHealthChecks("/healthz");
await app.RunAsync();