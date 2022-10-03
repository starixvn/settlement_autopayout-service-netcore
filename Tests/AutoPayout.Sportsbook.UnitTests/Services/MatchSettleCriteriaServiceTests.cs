using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class MatchSettleCriteriaServiceTests
{
    [Fact]
    public void CreateSettleCriteria_MatchIsHalftime_ReturnsRightActionName()
    {
        // Arrange
        var betStatusHalftimeService = Substitute.For<BetStatusHalftimeService>();
        var betStatusFullTimeService = Substitute.For<BetStatusFullTimeService>();
        var match = new Models.Match { TypeId = 1 };
        var matchChecking = new Models.MatchChecking();
        var matchSettleCriteriaService = new MatchSettleCriteriaServiceImp(betStatusHalftimeService, betStatusFullTimeService);

        // Act
        var actual = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);

        // Assert
        var expectedAction = "CAL-HT";
        Assert.Equal(expectedAction, actual.Action);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("FL:1-0", "FL:1-0")]
    public void CreateSettleCriteria_MatchIsHalftime_ReturnsRightExpectedBetStatus(string currentBetStatus, string expectedBetStatus)
    {
        // Arrange
        var betStatusHalftimeService = Substitute.For<BetStatusHalftimeService>();
        var betStatusFullTimeService = Substitute.For<BetStatusFullTimeService>();
        var match = new Models.Match { TypeId = 1, BetStatus = currentBetStatus };
        var matchChecking = new Models.MatchChecking();
        var matchSettleCriteriaService = new MatchSettleCriteriaServiceImp(betStatusHalftimeService, betStatusFullTimeService);

        // Act
        var actual = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);

        // Assert
        Assert.Equal(expectedBetStatus, actual.BetStatus);
    }

    [Fact]
    public void CreateSettleCriteria_MatchIsHalftimeAndIsExistHtFgLg_ReturnsExpectedBetStatusFromBetStatusHalftimeService()
    {
        // Arrange
        var betStatusHalftimeService = Substitute.For<BetStatusHalftimeService>();
        var betStatusFullTimeService = Substitute.For<BetStatusFullTimeService>();
        const string expectedBetStatus = "1FL:1-1;";
        var match = new Models.Match { TypeId = 1, BetStatus = "TestBetStatus", GoalRecord = "TestGoalRecord" };
        var matchChecking = new Models.MatchChecking { IsExistHtFgLg = true };

        betStatusHalftimeService.BetStatus(Arg.Any<string>(), Arg.Any<string>()).Returns("1FL:1-1;");
        var matchSettleCriteriaService = new MatchSettleCriteriaServiceImp(betStatusHalftimeService, betStatusFullTimeService);

        // Act
        var actual = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);

        // Assert
        Assert.Equal(expectedBetStatus, actual.BetStatus);
    }

    [Fact]
    public void CreateSettleCriteria_MatchIsHalftimeIsExistHtFgLgAndIsExistFgNg_ReturnsExpectedBetStatus()
    {
        // Arrange
        var betStatusHalftimeService = Substitute.For<BetStatusHalftimeService>();
        var betStatusFullTimeService = Substitute.For<BetStatusFullTimeService>();
        const string expectedBetStatus = "1FL:1-1;FL:x-0";
        var match = new Models.Match { TypeId = 1 };
        var matchChecking = new Models.MatchChecking { IsExistHtFgLg = true, IsExistFgNg = true };

        betStatusHalftimeService.AppendFullTimeBetStatus(Arg.Any<string>(), Arg.Any<string>())
            .Returns(expectedBetStatus);
        var matchSettleCriteriaService = new MatchSettleCriteriaServiceImp(betStatusHalftimeService, betStatusFullTimeService);

        // Act
        var actual = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);

        // Assert
        Assert.Equal(expectedBetStatus, actual.BetStatus);
    }

    [Fact]
    public void CreateSettleCriteria_MatchIsFullTime_ReturnsSettleFullTimeCriteria()
    {
        // Arrange
        var betStatusHalftimeService = Substitute.For<BetStatusHalftimeService>();
        var betStatusFullTimeService = Substitute.For<BetStatusFullTimeService>();
        const string expectedAction = "CAL-FT";
        const string expectedBetStatus = "1FL:1-2;";
        var match = new Models.Match { TypeId = 2, BetStatus = "1FL:1-2;" };
        var matchChecking = new Models.MatchChecking();
        var matchSettleCriteriaService = new MatchSettleCriteriaServiceImp(betStatusHalftimeService, betStatusFullTimeService);

        // Act
        var actual = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);

        // Assert
        Assert.Equal(expectedBetStatus, actual.BetStatus);
        Assert.Equal(expectedAction, actual.Action);
    }

    [Fact]
    public void CreateSettleCriteria_MatchIsFullTimeAndIsExistFgLg_ReturnsExpectedBetStatusFromBetStatusFullTimeService()
    {
        // Arrange
        var betStatusHalftimeService = Substitute.For<BetStatusHalftimeService>();
        var betStatusFullTimeService = Substitute.For<BetStatusFullTimeService>();
        const string expectedBetStatus = "1FL:1-1;FL:1-1;";
        var match = new Models.Match { TypeId = 2, BetStatus = "TestBetStatus", GoalRecord = "TestGoalRecord" };
        var matchChecking = new Models.MatchChecking { IsExistFgLg = true };

        betStatusFullTimeService.BetStatus(Arg.Any<string>(), Arg.Any<string>()).Returns("1FL:1-1;FL:1-1;");
        var matchSettleCriteriaService = new MatchSettleCriteriaServiceImp(betStatusHalftimeService, betStatusFullTimeService);

        // Act
        var actual = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);

        // Assert
        Assert.Equal(expectedBetStatus, actual.BetStatus);
    }

    [Fact]
    public void CreateSettleCriteria_MatchIsFullTimeAndSabaPenaltyShootout_ReturnsExpectedCriteria()
    {
        // Arrange
        var betStatusHalftimeService = Substitute.For<BetStatusHalftimeService>();
        var betStatusFullTimeService = Substitute.For<BetStatusFullTimeService>();
        var match = new Models.Match
        {
            TypeId = 2,
            BetStatus = "TestBetStatus",
            GoalRecord = "TestGoalRecord",
            LeagueGroupId = 42,
            LeagueId = 95730,
            ChildMatchType = 202
        };
        var matchChecking = new Models.MatchChecking { IsExistFgLg = false };
        var matchSettleCriteriaService = new MatchSettleCriteriaServiceImp(betStatusHalftimeService, betStatusFullTimeService);

        // Act
        var actual = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);

        // Assert
        Assert.Equal("CAL", actual.Action);
    }
}