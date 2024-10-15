using AutoMapper;
using AutoPayout.Core.Services;
using AutoPayout.Result.Services;
using AutoPayout.Sportsbook;
using AutoPayout.Sportsbook.Services;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.ScaleOut;
using Fanex.Settlement.ScaleOut.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Transport;
using SharpRaven;
using DirectPayoutEarly = DirectPayout.Early.Services;
using LogService = AutoPayout.Sportsbook.Services.LogService;

namespace Fanex.AutoPayout.Service;

public static class AutoPayoutInjector
{
    public static void InitializeAutoPayoutServices(this IServiceCollection service, IConfiguration configuration)
    {
        ConfigureAutoMapper(service);
        service.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        service.AddTransient<IMaintenanceService, MaintenanceServiceImp>();
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

        service.AddSingleton(new UMClientConfiguration().Initialize(configuration));
        service.AddSingleton<IUMCacheService, UMCacheService>();
        service.AddSingleton<IUMRestClient, UMRestClient>();
        service.AddSingleton<ONEbook.UM.Services.IMaintenanceService, ONEbook.UM.Services.MaintenanceService>();
        service.AddMemoryCache();

        StartUmSetting(configuration);
        StartLogSetting(configuration);

        ConfigureRedis(service, configuration);
    }

    public static void InitializeDirectPayoutServices(this IServiceCollection service)
    {
        service.AddSingleton<DirectPayoutEarly.DirectPayoutController, DirectPayoutEarly.DirectPayoutControllerImp>();
        service.AddSingleton<DirectPayoutEarly.MatchByLeagueService, DirectPayoutEarly.MatchByLeagueServiceImp>();
        service.AddSingleton<DirectPayoutEarly.MatchQueryService, DirectPayoutEarly.MatchQueryServiceImp>();
        service.AddSingleton<DirectPayoutEarly.MatchSettleController, DirectPayoutEarly.MatchSettleControllerImp>();
        service.AddSingleton<DirectPayoutEarly.MatchSettleService, DirectPayoutEarly.MatchSettleServiceImp>();
        service.AddSingleton<DirectPayoutEarly.MatchSettlingPoolService, DirectPayoutEarly.MatchSettlingPoolServiceImp>();
    }

    private static void StartLogSetting(IConfiguration configuration)
    {
        var logEngine = new List<ILogEngine>();

        var sentryEngineOptions = new SentryEngineOptions
        {
            Dsn = new Dsn(configuration.GetSection("SentryToken").Value)
        };

        logEngine.Add(new SentryLogging(sentryEngineOptions));

        LogManager.SetDefaultLogCategory(configuration.GetSection("SiteCategory").Value).Use(logEngine);
    }

    private static void StartUmSetting(IConfiguration configuration)
    {
        var umConfig = new UMClientConfiguration().Initialize(configuration);

        UMClientManager
            .UseConfig(umConfig)
            .UseCacheService(new UMCacheService(new CacheService()))
            .UseRestClient(new UMRestClient(umConfig));
    }

    private static void ConfigureRedis(IServiceCollection service, IConfiguration configuration)
    {
        var redisHost = configuration.GetSection("Redis.Server").Value;
        var redisPort = configuration.GetSection("Redis.Port").Value;
        var redisPassword = configuration.GetSection("Redis.Password").Value;
        var redisDatabase = configuration.GetSection("Redis.DefaultDatabase").Value;
        service.AddSingleton<ICache>(new RedisCache(new RedisCacheOptions
        {
            ConnectionString = $"{redisHost}:{redisPort},password={redisPassword},defaultDatabase={redisDatabase}",
            ApplicationName = configuration.GetSection("Redis.ApplicationName").Value,
        }));
        service.AddSingleton<ICacheService>(new CacheService());
    }

    private static void ConfigureAutoMapper(IServiceCollection service)
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(typeof(AutoMapperProfile));
            mc.AddProfile(typeof(ScaleOutMappingProfile));
        });

        var mapper = mapperConfig.CreateMapper();
        service.AddSingleton(mapper);
    }
}