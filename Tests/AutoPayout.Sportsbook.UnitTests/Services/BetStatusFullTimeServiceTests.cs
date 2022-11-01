using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class BetStatusFullTimeServiceTests
{
    [Theory]
    [InlineData("1FL:1-0;", "1FL:1-0;FL:1-2;")]
    [InlineData("", "FL:1-2;")]
    public void BetStatus_WithCurrentBetStatusAndNewFullTimeFgLg_ReturnsRightExpectedBetStatus(string currentBetStatus, string expectedBetStatus)
    {
        // Arrange
        var goalRecord = ";";
        var betStatusService = Substitute.For<BetStatusService>();
        betStatusService.GenerateBetStatus("FL", string.Empty).Returns("FL:1-2;");

        var service = new BetStatusFullTimeServiceImp(betStatusService);
        // Act
        var actual = service.BetStatus(currentBetStatus, goalRecord);

        // Assert
        Assert.Equal(expectedBetStatus, actual);
    }
}