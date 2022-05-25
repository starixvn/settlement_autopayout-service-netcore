using AutoPayout.Sportsbook.Services;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using AutoPayout.Sportsbook.Models;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Microsoft.Extensions.Options;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class MatchByLeagueServiceTest
    {
        private List<Match> CreateMatchesWithLeagueOne(params int[] matchIds)
        {
            var leagueId = 1;
            return matchIds.Select(i => new Match { MatchId = i, LeagueId = leagueId }).ToList();
        }

        private List<Match> CreateMatchesWithLeagueTwo(params int[] matchIds)
        {
            var leagueId = 2;
            return matchIds.Select(i => new Match { MatchId = i, LeagueId = leagueId }).ToList();
        }

        private List<Match> CreateMatchesWithLeagueThree(params int[] matchIds)
        {
            var leagueId = 3;
            return matchIds.Select(i => new Match { MatchId = i, LeagueId = leagueId }).ToList();
        }

        private List<Match> CreateMatches
        {
            get
            {
                var matchesOne = CreateMatchesWithLeagueOne(1);
                var matchesTwo = CreateMatchesWithLeagueTwo(2);
                var matchesThree = CreateMatchesWithLeagueThree(3);

                var list = new List<Match>();
                list.AddRange(matchesOne);
                list.AddRange(matchesTwo);
                list.AddRange(matchesThree);

                return list;
            }
        }

        [Fact]
        public async Task GetMatches_ExcludeLeagueIds_ReturnsRightMatches()
        {
            // Arrange
            var cache = new RedisCache(new RedisCacheOptions
            {
                ConnectionString = $"ha.nexdev.net:16379,password=foo1@bared,defaultDatabase=11",
                ApplicationName = "Settlement-LOCAL"
            });

            var settings = new AppSettingsKey()
            {
                RetryInterval = 1000,
                RetryCount = 1,
                EnvironmentName = "Local"
            };
            var appSettingsOptions = Options.Create(settings);
            var matchQueryService = Substitute.For<MatchQueryService>();
            var matchByLeagueService = new MatchByLeagueServiceImp(matchQueryService, cache, appSettingsOptions);
            const string KEY_LEAGUEID = "Settlement-AutoPayout-AutoInstant-LeagueId";
            const int CACHE_EXPIRED_IN_DAYS = 1095;
            var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(CACHE_EXPIRED_IN_DAYS));
            cache.Set(KEY_LEAGUEID, 1, cachePolicy);
            matchQueryService.GetWaitingMatches().Returns(CreateMatches);

            // Act
            var matches = await matchByLeagueService.GetMatches();

            // Assert
            Assert.Equal(3, matches.Count);
        }
    }
}