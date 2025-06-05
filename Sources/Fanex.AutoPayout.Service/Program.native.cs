using AutoPayout.Core.Models;
using DirectPayout.Early;
using Fanex.AutoPayout.Service;
using Fanex.AutoPayout.Service.Services;
using Fanex.Data.SqlServer;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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

var app = builder.Build();

app.Run(async context =>
{
    await context.Response.WriteAsync("OK");
});

await app.RunAsync();