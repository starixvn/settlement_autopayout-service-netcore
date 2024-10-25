using DirectPayout.Early.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DirectPayout.Early;

public static class DirectPayoutEarlyExtensions
{
    public static WebApplicationBuilder AddDirectPayoutEarlyModule(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<DirectPayoutController, DirectPayoutControllerImp>()
            .AddSingleton<MatchService, MatchServiceImp>()
            .AddSingleton<MatchSettleController, MatchSettleControllerImp>()
            .AddSingleton<MatchSettleService, MatchSettleServiceImp>()
            .AddSingleton<MatchSettlingPoolService, MatchSettlingPoolServiceImp>();

        return builder;
    }
}
