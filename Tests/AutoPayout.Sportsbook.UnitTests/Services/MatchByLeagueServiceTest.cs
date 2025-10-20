using AutoPayout.Core.Constants;
using AutoPayout.Core.Models;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;
using Fanex.Caching;
using Microsoft.Extensions.Options;
using NSubstitute.ExceptionExtensions;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class MatchByLeagueServiceTest
{
    private readonly ICache _cache;
    private readonly MatchQueryService _matchQueryService;
    private MatchByLeagueServiceImp _matchByLeagueService;

    public MatchByLeagueServiceTest()
    {
        _cache = Substitute.For<ICache>();
        _matchQueryService = Substitute.For<MatchQueryService>();
    }

    private MatchByLeagueServiceImp CreateService(string environmentName = "Local")
    {
        var settings = new AppSettingsKey
        {
            RetryInterval = 1000,
            RetryCount = 1,
            EnvironmentName = environmentName
        };
        var appSettingsOptions = Options.Create(settings);
        return new MatchByLeagueServiceImp(_matchQueryService, _cache, appSettingsOptions);
    }

    private void SetLeague(int leagueId, bool exists = true)
    {
        _cache.TryGetAsync<int>(CacheKey.LeagueId)
            .Returns(Task.FromResult((exists, leagueId)));
    }

    private static List<Match> CreateMatchesWithLeagues(params (int matchId, int leagueId)[] matchData)
    {
        return matchData.Select(data => new Match { MatchId = data.matchId, LeagueId = data.leagueId }).ToList();
    }

    private static List<Match> CreateMatches => CreateMatchesWithLeagues((1, 1), (2, 2), (3, 3));

    #region Environment-based filtering tests

    [Fact]
    public async Task GetMatches_LocalEnvironment_ReturnsAllMatches()
    {
        // Arrange
        SetLeague(1);
        _matchByLeagueService = CreateService("Local");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(3, matches.Count);
    }

    [Fact]
    public async Task GetMatches_DevEnvironment_ReturnsAllMatches()
    {
        // Arrange
        SetLeague(1);
        _matchByLeagueService = CreateService("Dev");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(3, matches.Count);
    }

    [Fact]
    public async Task GetMatches_UATEnvironment_ReturnsMatchingLeague()
    {
        // Arrange
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(matches);
        Assert.All(matches, m => Assert.Equal(1, m.LeagueId));
    }

    [Fact]
    public async Task GetMatches_UATEnvironment_NoMatchingLeague_ReturnsEmpty()
    {
        // Arrange
        SetLeague(5); // League that doesn't exist in test data
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Empty(matches);
    }

    [Fact]
    public async Task GetMatches_PROEnvironment_ExcludesSpecificLeague()
    {
        // Arrange
        SetLeague(1);
        _matchByLeagueService = CreateService("PRO");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(2, matches.Count);
        Assert.All(matches, m => Assert.NotEqual(1, m.LeagueId));
        Assert.Contains(matches, m => m.LeagueId == 2);
        Assert.Contains(matches, m => m.LeagueId == 3);
    }

    [Fact]
    public async Task GetMatches_PROEnvironment_NoLeagueFilter_ReturnsAllMatches()
    {
        // Arrange
        SetLeague(0); // No league filter
        _matchByLeagueService = CreateService("PRO");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(3, matches.Count);
    }

    [Fact]
    public async Task GetMatches_PROEnvironment_NegativeLeagueFilter_ReturnsAllMatches()
    {
        // Arrange
        SetLeague(-1); // Invalid league filter
        _matchByLeagueService = CreateService("PRO");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(3, matches.Count);
    }

    #endregion

    #region Cache scenarios

    [Fact]
    public async Task GetMatches_CacheException_HandlesGracefully()
    {
        // Arrange
        _cache.TryGetAsync<int>(CacheKey.LeagueId).Throws(new Exception("Cache error"));
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _matchByLeagueService.GetMatches());
    }

    #endregion

    #region MatchType assignment tests

    [Fact]
    public async Task GetMatches_PenaltyShootoutHdp_SetsCorrectMatchType()
    {
        // Arrange
        var matches = new List<Match>
        {
            new Match { MatchId = 1, LeagueId = 1, ChildMatchType = MatchTypes.PenaltyShootoutHdp }
        };
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(matches);

        // Act
        var result = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchTypes.PenaltyShootout, result[0].MatchType);
    }

    [Fact]
    public async Task GetMatches_PenaltyShootoutOu_SetsCorrectMatchType()
    {
        // Arrange
        var matches = new List<Match>
        {
            new Match { MatchId = 1, LeagueId = 1, ChildMatchType = MatchTypes.PenaltyShootoutOu }
        };
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(matches);

        // Act
        var result = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchTypes.PenaltyShootout, result[0].MatchType);
    }

    [Fact]
    public async Task GetMatches_ExtraTime_SetsCorrectMatchType()
    {
        // Arrange
        var matches = new List<Match>
        {
            new Match { MatchId = 1, LeagueId = 1, ChildMatchType = MatchTypes.ExtraTime }
        };
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(matches);

        // Act
        var result = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchTypes.ExtraTime, result[0].MatchType);
    }

    [Fact]
    public async Task GetMatches_TotalGoalsMinutes_SetsCorrectMatchType()
    {
        // Arrange
        var matches = new List<Match>
        {
            new Match
            {
                MatchId = 1,
                LeagueId = 1,
                ChildLeagueType = 1018,
                MainType = 19,
                SubType = 6200
            }
        };
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(matches);

        // Act
        var result = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchTypes.TotalGoalsMinutes, result[0].MatchType);
    }

    [Fact]
    public async Task GetMatches_InjuryTimeFirstHalf_SetsCorrectMatchType()
    {
        // Arrange
        var matches = new List<Match>
        {
            new Match
            {
                MatchId = 1,
                LeagueId = 1,
                ChildLeagueType = 1016,
                MainType = 17,
                SubType = 900
            }
        };
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(matches);

        // Act
        var result = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchTypes.InjuryTimeFirstHalf, result[0].MatchType);
    }

    [Fact]
    public async Task GetMatches_InjuryTimeSecondHalf_SetsCorrectMatchType()
    {
        // Arrange
        var matches = new List<Match>
        {
            new Match
            {
                MatchId = 1,
                LeagueId = 1,
                ChildLeagueType = 1017,
                MainType = 18,
                SubType = 900
            }
        };
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(matches);

        // Act
        var result = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(result);
        Assert.Equal(MatchTypes.InjuryTimeSecondHalf, result[0].MatchType);
    }

    [Fact]
    public async Task GetMatches_UnknownMatchType_SetsZero()
    {
        // Arrange
        var matches = new List<Match>
        {
            new Match
            {
                MatchId = 1,
                LeagueId = 1,
                ChildMatchType = 999, // Unknown
                ChildLeagueType = 999,
                MainType = 999,
                SubType = 999
            }
        };
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(matches);

        // Act
        var result = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(result);
        Assert.Equal(0, result[0].MatchType);
    }

    [Fact]
    public async Task GetMatches_MixedMatchTypes_SetsCorrectTypes()
    {
        // Arrange
        var matches = new List<Match>
        {
            new Match
            {
                MatchId = 1,
                LeagueId = 1,
                ChildMatchType = MatchTypes.PenaltyShootoutHdp
            },
            new Match
            {
                MatchId = 2,
                LeagueId = 1,
                ChildMatchType = MatchTypes.ExtraTime
            },
            new Match
            {
                MatchId = 3,
                LeagueId = 1,
                ChildLeagueType = 1018,
                MainType = 19,
                SubType = 6200
            }
        };
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(matches);

        // Act
        var result = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(MatchTypes.PenaltyShootout, result[0].MatchType);
        Assert.Equal(MatchTypes.ExtraTime, result[1].MatchType);
        Assert.Equal(MatchTypes.TotalGoalsMinutes, result[2].MatchType);
    }

    #endregion

    #region Edge cases and error handling

    [Fact]
    public async Task GetMatches_EmptyMatchList_ReturnsEmpty()
    {
        // Arrange
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(new List<Match>());

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Empty(matches);
    }


    [Fact]
    public async Task GetMatches_MatchQueryServiceThrows_PropagatesException()
    {
        // Arrange
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _matchByLeagueService.GetMatches());
    }

    [Fact]
    public async Task GetMatches_CallsMatchQueryServiceOnce()
    {
        // Arrange
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        await _matchByLeagueService.GetMatches();

        // Assert
        await _matchQueryService.Received(1).GetWaitingMatches();
    }

    [Fact]
    public async Task GetMatches_CallsCacheOnce()
    {
        // Arrange
        SetLeague(1);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        await _matchByLeagueService.GetMatches();

        // Assert
        await _cache.Received(1).TryGetAsync<int>(CacheKey.LeagueId);
    }

    #endregion

    #region Performance and boundary tests

    [Fact]
    public async Task GetMatches_LargeMatchList_HandlesEfficiently()
    {
        // Arrange
        var largeMatchList = Enumerable.Range(1, 1000)
            .Select(i => new Match { MatchId = i, LeagueId = i % 10 })
            .ToList();

        SetLeague(5);
        _matchByLeagueService = CreateService("UAT");
        _matchQueryService.GetWaitingMatches().Returns(largeMatchList);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(100, matches.Count); // Every 10th match should have LeagueId = 5
        Assert.All(matches, m => Assert.Equal(5, m.LeagueId));
    }

    [Theory]
    [InlineData("PRO")]
    [InlineData("UAT")]
    public async Task GetMatches_EnvironmentNameCaseInsensitive_WorksCorrectly(string environmentName)
    {
        // Arrange
        SetLeague(1);
        _matchByLeagueService = CreateService(environmentName);
        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        if (environmentName.ToUpper() == "PRO")
        {
            Assert.Equal(2, matches.Count);
            Assert.All(matches, m => Assert.NotEqual(1, m.LeagueId));
        }
        else if (environmentName.ToUpper() == "UAT")
        {
            Assert.Single(matches);
            Assert.All(matches, m => Assert.Equal(1, m.LeagueId));
        }
    }

    #endregion

    #region Legacy test compatibility

    [Fact]
    public async Task GetMatches_NotExistLeagueTest_ReturnsAllMatches()
    {
        // Arrange
        _cache.TryGetAsync<int>(CacheKey.LeagueId)
            .Returns(Task.FromResult((false, 0)));

        var settings = new AppSettingsKey()
        {
            RetryInterval = 1000,
            RetryCount = 1,
            EnvironmentName = "Local"
        };
        var appSettingsOptions = Options.Create(settings);

        _matchByLeagueService = new MatchByLeagueServiceImp(_matchQueryService, _cache, appSettingsOptions);

        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

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

        _matchByLeagueService = new MatchByLeagueServiceImp(_matchQueryService, _cache, appSettingsOptions);

        _matchQueryService.GetWaitingMatches().Returns(CreateMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Single(matches);
        Assert.All(matches, m => Assert.Equal(1, m.LeagueId));
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
        var createMatches = CreateMatches;
        var appSettingsOptions = Options.Create(settings);

        _matchByLeagueService = new MatchByLeagueServiceImp(_matchQueryService, _cache, appSettingsOptions);

        _matchQueryService.GetWaitingMatches().Returns(createMatches);

        // Act
        var matches = await _matchByLeagueService.GetMatches();

        // Assert
        Assert.Equal(2, matches.Count);
        Assert.All(matches, m => Assert.NotEqual(1, m.LeagueId));
    }

    #endregion
}