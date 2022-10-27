using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;
using Fanex.Caching;
using Fanex.Caching.Redis;
using Microsoft.Extensions.Options;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class MatchByLeagueServiceTest
{
    private readonly ICache cache;

    private readonly MatchQueryService matchQueryService;

    private MatchByLeagueService matchByLeagueService;

    public MatchByLeagueServiceTest()
    {
        cache = new RedisCache(new RedisCacheOptions
        {
            ConnectionString = "10.18.200.131:16379,password=foo1@bared,defaultDatabase=11",
            ApplicationName = "Settlement-LOCAL"
        });

        matchQueryService = Substitute.For<MatchQueryService>();
    }

    private void SetLeague(int leagueId)
    {
        const string keyIncludeLeagueIds = "Settlement-AutoPayout-AutoInstant-LeagueId";
        const int cacheExpiredInDays = 1095;
        var cachePolicy = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(cacheExpiredInDays));
        cache.Set(keyIncludeLeagueIds, leagueId, cachePolicy);
    }

    private static List<Match> CreateMatchesWithLeagueOne(params int[] matchIds)
    {
        const int leagueId = 1;
        return matchIds.Select(i => new Match { MatchId = i, LeagueId = leagueId }).ToList();
    }

    private static List<Match> CreateMatchesWithLeagueTwo(params int[] matchIds)
    {
        const int leagueId = 2;
        return matchIds.Select(i => new Match { MatchId = i, LeagueId = leagueId }).ToList();
    }

    private static List<Match> CreateMatchesWithLeagueThree(params int[] matchIds)
    {
        const int leagueId = 3;
        return matchIds.Select(i => new Match { MatchId = i, LeagueId = leagueId }).ToList();
    }

    private static List<Match> CreateMatches
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
    public async Task GetMatches_NotExistLeagueTest_ReturnsAllMatches()
    {
        // Arrange
        var settings = new AppSettingsKey()
        {
            RetryInterval = 1000,
            RetryCount = 1,
            EnvironmentName = "Local"
        };
        var appSettingsOptions = Options.Create(settings);

        matchByLeagueService = new MatchByLeagueServiceImp(matchQueryService, cache, appSettingsOptions);

        matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(3, matches.Count);
    }

    [Fact]
    public async Task GetMatches_ExcludeLeagueIdAndInUAT_ReturnsOneMatches()
    {
        // Arrange
        SetLeague(1);

        var settings = new AppSettingsKey()
        {
            RetryInterval = 1000,
            RetryCount = 1,
            EnvironmentName = "UAT"
        };
        var appSettingsOptions = Options.Create(settings);

        matchByLeagueService = new MatchByLeagueServiceImp(matchQueryService, cache, appSettingsOptions);

        matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        await Task.Delay(2000);
        var matches = await matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(matches);
    }

    [Fact]
    public async Task GetMatches_ExcludeLeagueIdAndInPRO_ReturnsTwoMatches()
    {
        // Arrange
        SetLeague(1);

        var settings = new AppSettingsKey()
        {
            RetryInterval = 1000,
            RetryCount = 1,
            EnvironmentName = "PRO"
        };
        var appSettingsOptions = Options.Create(settings);

        matchByLeagueService = new MatchByLeagueServiceImp(matchQueryService, cache, appSettingsOptions);

        matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        await Task.Delay(3000);
        var matches = await matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(2, matches.Count);
    }
}