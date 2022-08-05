using AutoMapper;
using AutoPayout.Result.Services;
using AutoPayout.Sportsbook;
using AutoPayout.Sportsbook.Services;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Fanex.Data.Repository;
using Fanex.Settlement.ScaleOut;
using Fanex.Settlement.ScaleOut.Services;
using Kaden.XmlConverter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Transport;

namespace Fanex.AutoPayout.Service
{
    public static class AutoPayoutInjector
    {
        public static void InitializeServices(this IServiceCollection service, IConfiguration configuration)
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
            service.AddSingleton<MatchSettleCriteriaService, MatchSettleCriteriaServiceImp>();
            service.AddSingleton<AutoPayoutResultService, AutoPayoutResultServiceImp>();
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

            service.AddSingleton(new UMClientConfiguration().Initialize(configuration));
            service.AddSingleton<IUMCacheService, UMCacheService>();
            service.AddSingleton<IUMRestClient, UMRestClient>();
            service.AddSingleton<ONEbook.UM.Services.IMaintenanceService, ONEbook.UM.Services.MaintenanceService>();
            ConfigureRedis(service, configuration);
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
}