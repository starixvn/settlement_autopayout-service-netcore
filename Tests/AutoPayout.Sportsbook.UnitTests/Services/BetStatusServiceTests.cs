using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class BetStatusServiceTests
{
    private readonly BetStatusService service;

    public BetStatusServiceTests()
    {
        service = new BetStatusServiceImp();
    }

    [Theory]
    [InlineData("", "FL:0-0;")]
    [InlineData("AHAH", "FL:2-1;")]
    [InlineData("AAAA", "FL:2-2;")]
    [InlineData("HAAA", "FL:1-2;")]
    [InlineData("HHHH", "FL:1-1;")]
    public void GenerateBetStatus_WithGoalRecord_ReturnsRightExpectedBetStatus(string goalRecord, string expectedBetStatus)
    {
        // Act
        var actual = service.GenerateBetStatus("FL", goalRecord);

        // Assert
        Assert.Equal(expectedBetStatus, actual);
    }


    [Fact]
    public void GenerateBetStatus_GoalRecordEmpty_ReturnsEmpty()
    {
        // Act
        var actual = service.GenerateBetStatus(string.Empty);

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public void GenerateBetStatus_GoalRecord_ReturnsExpectedBetStatus()
    {
        // Arrange
        const string goalRecord = "AH;HHH";
        const string expectedBetStatus = "FL:2-0;";

        // Act
        var actual = service.GenerateBetStatus(goalRecord);

        // Assert
        Assert.Equal(expectedBetStatus, actual);
    }
}