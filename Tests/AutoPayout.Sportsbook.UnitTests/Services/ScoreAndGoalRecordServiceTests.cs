using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class ScoreAndGoalRecordServiceTests
{
    private readonly ScoreAndGoalRecordService service;

    public ScoreAndGoalRecordServiceTests()
    {
        service = new ScoreAndGoalRecordServiceImp();
    }

    [Theory]
    [InlineData(null, null, ";")]
    public void IsConsistentHalftime_ScoreAndGoalRecordValue_ReturnsFalse(
        int? homeScore, int? awayScore, string goalRecord)
    {
        // Arrange
        var match = new Match
        {
            HtHomeScore = homeScore,
            HtAwayScore = awayScore,
            GoalRecord = goalRecord
        };

        // Act
        var isConsistent = service.IsConsistentHalftime(match);

        // Assert
        Assert.False(isConsistent);
    }

    [Theory]
    [InlineData(0, 0, "")]
    [InlineData(0, 0, ";")]
    [InlineData(1, 0, "H;")]
    [InlineData(0, 1, "a;")]
    [InlineData(2, 1, "Hha;")]
    [InlineData(2, 3, "HaaAh;hahaaaa")]
    public void IsConsistentHalftime_ScoreAndGoalRecordValue_ReturnsTrue(
        int? homeScore, int? awayScore, string goalRecord)
    {
        // Arrange
        var match = new Match
        {
            HtHomeScore = homeScore,
            HtAwayScore = awayScore,
            GoalRecord = goalRecord
        };

        // Act
        var isConsistent = service.IsConsistentHalftime(match);

        // Assert
        Assert.True(isConsistent);
    }

    [Theory]
    [InlineData(0, 0, "")]
    [InlineData(0, 0, ";")]
    [InlineData(1, 0, ";H")]
    [InlineData(0, 1, ";a")]
    [InlineData(2, 1, ";Hha")]
    [InlineData(2, 3, "haHAA")]
    public void IsConsistentFullTime_ScoreAndGoalRecordValue_ReturnsTrue(
       int? homeScore, int? awayScore, string goalRecord)
    {
        // Arrange
        var match = new Match
        {
            FinalHomeScore = homeScore,
            FinalAwayScore = awayScore,
            GoalRecord = goalRecord
        };

        // Act
        var isConsistent = service.IsConsistentFullTime(match);

        // Assert
        Assert.True(isConsistent);
    }

    [Theory]
    [InlineData(";Hha")]
    [InlineData(";")]
    public void IsExistedSemiColon_GoalRecordValue_ReturnsTrue(string goalRecord)
    {
        // Act
        var isExistedSemiColon = service.IsExistedSemiColon(goalRecord);

        // Assert
        Assert.True(isExistedSemiColon);
    }
}