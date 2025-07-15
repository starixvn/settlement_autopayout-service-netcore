using AutoPayout.Result.Services;
using AutoPayout.Sportsbook;
using AutoPayout.Sportsbook.Services;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.ScaleOut;
using Fanex.Settlement.ScaleOut.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LogService = AutoPayout.Sportsbook.Services.LogService;

namespace Fanex.AutoPayout.Service;

public static class AutoPayoutInjector
{
    public static void InitializeAutoPayoutServices(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        service.AddSingleton<IDynamicRepository, DynamicRepository>();
        service.AddSingleton<AutoPayoutController, AutoPayoutControllerImp>();
        service.AddSingleton<ScoreMatcherController, ScoreMatcherControllerImp>();
        service.AddSingleton<ActionResourceService, ActionResourceServiceImp>();
        service.AddSingleton<FirstGoalService, FirstGoalServiceImp>();
        service.AddSingleton<GoalRecordService, GoalRecordServiceImp>();
        service.AddSingleton<MatchSettlingPoolService, MatchSettlingPoolServiceImp>();
        service.AddSingleton<MatchQueryService, MatchQueryServiceImp>();
        service.AddSingleton<XmlConvertService, XmlConvertServiceImp>();
        service.AddSingleton<AutoPayoutController, AutoPayoutControllerImp>();
        service.AddSingleton<MatchSettleController, MatchSettleControllerImp>();
        service.AddSingleton<MatchSettleService, MatchSettleServiceImp>();
        service.AddSingleton<ScoreMatcherController, ScoreMatcherControllerImp>();
        service.AddSingleton<ScoreService, ScoreServiceImp>();
        service.AddSingleton<SettleLogService, SettleLogServiceImp>();
        service.AddSingleton<CreateCriteriaService, CreateCriteriaServiceImp>();
        service.AddSingleton<InsertInfoService, InsertInfoServiceImp>();
        service.AddSingleton<ValidationGoalRecordService, ValidationGoalRecordServiceImp>();
        service.AddSingleton<ValidationSettingMatchService, ValidationSettingMatchServiceImp>();
        service.AddSingleton<ValidationStatusMatchService, ValidationStatusMatchServiceImp>();
        service.AddSingleton<ValidationScoreService, ValidationScoreServiceImp>();
        service.AddSingleton<BetStatusHalftimeService, BetStatusHalftimeServiceImp>();
        service.AddSingleton<BetStatusFullTimeService, BetStatusFullTimeServiceImp>();
        service.AddSingleton<BetStatusService, BetStatusServiceImp>();
        service.AddSingleton<ScoreAndGoalRecordService, ScoreAndGoalRecordServiceImp>();
        service.AddSingleton<MatchByLeagueService, MatchByLeagueServiceImp>();
        service.AddSingleton<OtherInfoService, OtherInfoServiceImp>();
        service.AddTransient<ScaleOutSettleService, ScaleOutSettleServiceImp>();
        service.AddTransient<GeneralService, GeneralServiceImp>();
        service.AddTransient<LogService, LogServiceImp>();
    }

    public static void AddConfigRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("Redis:ConnectionString");
        var applicationName = configuration.GetValue<string>("Redis:ApplicationName");

        services.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
        {
            ConnectionString = connectionString,
            ApplicationName = applicationName,
        }));
        services.AddSingleton<ICacheService>(new CacheService());
    }

    public static void AddAutoMapper(this IServiceCollection service)
    {
        service.AddAutoMapper(mc =>
        {
            mc.AddProfile<AutoMapperProfile>();
            mc.AddProfile<ScaleOutMappingProfile>();
        });
    }
}