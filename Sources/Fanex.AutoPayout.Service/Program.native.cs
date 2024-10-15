using AutoPayout.Core.Models;
using Fanex.AutoPayout.Service;
using Fanex.AutoPayout.Service.Services;
using Fanex.Data.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFanexSql(builder.Configuration);

builder.Services.Configure<AppSettingsKey>(builder.Configuration);

builder.Services.InitializeAutoPayoutServices(builder.Configuration);
builder.Services.InitializeDirectPayoutServices();

//builder.Services.AddHostedService<AutoPayoutService>();
builder.Services.AddHostedService<DirectPayoutEarlyService>();

var app = builder.Build();

app.Run(async context =>
{
    await context.Response.WriteAsync("OK");
});

app.Run();