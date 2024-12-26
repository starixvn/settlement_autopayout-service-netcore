using AutoPayout.Core.Constants;
using AutoPayout.Core.Models;
using DirectPayout.Early.Models;
using DirectPayout.Early.Services;
using Fanex.Caching;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DirectPayout.Early.Tests.Services;

public class MatchServiceTests
{
    private readonly ResultDetailService resultDetailService;
    private readonly ICache cache;
    private readonly MatchServiceImp matchService;
    private readonly IOptions<AppSettingsKey> appSettingsKey;

    public MatchServiceTests()
    {
        resultDetailService = Substitute.For<ResultDetailService>();
        cache = Substitute.For<ICache>();
        appSettingsKey = Options.Create(new AppSettingsKey { EnvironmentName = "PRO" });

        matchService = new MatchServiceImp(resultDetailService, cache, appSettingsKey);
    }

    [Fact]
    public async Task GetMatches_ShouldReturnFilteredMatches_WhenEnvironmentIsUAT()
    {
        // Arrange
        const int leagueIds = 4;
        cache.TryGetAsync<int>(CacheKey.LeagueId).Returns(Task.FromResult((true, leagueIds)));

        var resultDetails = new List<ResultDetail>
    {
        new() { MatchId = 1, LeagueId = 1 },
        new() { MatchId = 2, LeagueId = 2 },
        new() { MatchId = 3, LeagueId = 4 }
    };

        resultDetailService.GetResultDetailsAsync().Returns(resultDetails);

        // Act
        var matches = await matchService.GetMatches();

        // Assert
        Assert.Equal(2, matches.Count);
        Assert.True(matches.All(m => new[] { 1, 2 }.Contains(m.LeagueId)));
    }

    [Fact]
    public async Task GetMatches_ShouldReturnUnfilteredMatches_WhenEnvironmentIsPRO()
    {
        // Arrange
        appSettingsKey.Value.EnvironmentName = "PRO";
        const int leagueIds = 1;
        cache.TryGetAsync<int>(CacheKey.LeagueId).Returns(Task.FromResult((true, leagueIds)));

        var resultDetails = new List<ResultDetail>
    {
        new() { MatchId = 1, LeagueId = 1 },
        new() { MatchId = 2, LeagueId = 1 },
        new() { MatchId = 3, LeagueId = 4 }
    };

        resultDetailService.GetResultDetailsAsync().Returns(resultDetails);

        // Act
        var matches = await matchService.GetMatches();

        // Assert
        Assert.Single(matches);
        Assert.Single(matches, m => m.LeagueId == 4);
    }
}