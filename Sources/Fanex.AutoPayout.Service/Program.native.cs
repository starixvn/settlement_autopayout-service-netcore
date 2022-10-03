using AutoPayout.Sportsbook.Models;
using Fanex.AutoPayout.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettingsKey>(builder.Configuration)
                    .AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettingsKey>>().Value);

builder.Services.AddHostedService<AutoPayoutService>();

builder.Services.InitializeServices(builder.Configuration);

var app = builder.Build();



#if DEBUG
app.Run();
#else
app.Run("http://0.0.0.0:5000")
#endif