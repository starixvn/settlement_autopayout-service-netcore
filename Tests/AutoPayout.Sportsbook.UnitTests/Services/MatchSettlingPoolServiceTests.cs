using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class MatchSettlingPoolServiceTests
{
    [Fact]
    public async Task FetchNewMatch_Twice_OnlyAddNewMatchToPool()
    {
        // Arrange
        var matchByLeagueService = Substitute.For<MatchByLeagueService>();
        var matchListingService = new MatchSettlingPoolServiceImp(matchByLeagueService);

        // Act1
        matchByLeagueService.GetMatches().Returns(new List<Match> { new Match { MatchId = 1, TypeId = 1 }, new Match { MatchId = 2, TypeId = 1 } });
        await matchListingService.PeekNewMatch();

        // Act2
        matchByLeagueService.GetMatches().Returns(new List<Match> { new Match { MatchId = 2, TypeId = 1 }, new Match { MatchId = 3, TypeId = 1 } });
        await matchListingService.PeekNewMatch();

        // Assert
        Assert.Equal(3, matchListingService.PoolOfMatches.Count);
    }

    private static List<Match> CreateMatches(params int[] matchIds)
    {
        return matchIds.Select(i => new Match { MatchId = i, TypeId = 1 }).ToList();
    }

    [Fact]
    public async Task Remove_ExistedMatchId_ExistingMatchIsRemove()
    {
        // Arrange
        var matchByLeagueService = Substitute.For<MatchByLeagueService>();
        var matchListingService = new MatchSettlingPoolServiceImp(matchByLeagueService);
        var matches = CreateMatches(1, 2);
        matchByLeagueService.GetMatches().Returns(matches);

        // Act1
        await matchListingService.PeekNewMatch();
        matchListingService.Remove(matches[0]);

        // Assert
        Assert.Single(matchListingService.PoolOfMatches);
    }

    [Fact]
    public async Task Remove_NotExistedMatchId_NoChange()
    {
        // Arrange
        var matchByLeagueService = Substitute.For<MatchByLeagueService>();
        var matchListingService = new MatchSettlingPoolServiceImp(matchByLeagueService);
        var matches = CreateMatches(1, 2);
        matchByLeagueService.GetMatches().Returns(matches);

        // Act1
        await matchListingService.PeekNewMatch();
        matchListingService.Remove(new Match());

        // Assert
        Assert.Equal(2, matchListingService.PoolOfMatches.Count);
    }
}