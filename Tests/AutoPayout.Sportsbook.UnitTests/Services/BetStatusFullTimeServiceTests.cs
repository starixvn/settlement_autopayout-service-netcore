using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class BetStatusFullTimeServiceTests
{
    [Theory]
    [InlineData("1FL:1-0;", "1FL:1-0;FL:1-2;")]
    [InlineData("", "FL:1-2;")]
    [InlineData("Refund;FL:1-0;", "Refund;FL:1-2;")]
    public void BetStatus_WithCurrentBetStatusAndNewFullTimeFgLg_ReturnsRightExpectedBetStatus(string currentBetStatus, string expectedBetStatus)
    {
        // Arrange
        var betStatusService = Substitute.For<BetStatusService>();
        betStatusService.GenerateBetStatus(Arg.Any<string>(), Arg.Any<string>()).Returns("FL:1-2;");

        var service = new BetStatusFullTimeServiceImp(betStatusService);
        // Act
        var actual = service.BetStatus(currentBetStatus, ";");

        // Assert
        Assert.Equal(expectedBetStatus, actual);
    }
}