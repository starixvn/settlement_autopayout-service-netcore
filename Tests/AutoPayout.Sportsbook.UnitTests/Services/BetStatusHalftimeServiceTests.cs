using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class BetStatusHalftimeServiceTests
{
    [Theory]
    [InlineData("", "1FL:1-2;")]
    [InlineData("FL:2-0;", "1FL:1-2;FL:2-0;")]
    [InlineData("1FL:1-0;FL:1-0;", "1FL:1-2;FL:1-0;")]
    [InlineData("FL:1-0;1FL:1-0;", "1FL:1-2;FL:1-0;")]
    public void BetStatus_WithCurrentBetStatusAndNewHalftimeFgLg_ReturnsRightExpectedBetStatus(string currentBetStatus, string expectedBetStatus)
    {
        // Arrange
        var betStatusService = Substitute.For<BetStatusService>();
        var service = new BetStatusHalftimeServiceImp(betStatusService);
        betStatusService.GenerateBetStatus("1FL", string.Empty).Returns("1FL:1-2;");

        // Act
        var actual = service.BetStatus(currentBetStatus, string.Empty);

        // Assert
        Assert.Equal(expectedBetStatus, actual);
    }

    [Theory]
    [InlineData("1FL:1-0;", "", "1FL:1-0;")]
    [InlineData("Refund;1FL:1-0;", "", "Refund;1FL:1-0;")]
    [InlineData("", "FL:x-0;", "FL:x-0;")]
    [InlineData("1FL:1-0;", "FL:2-0;", "1FL:1-0;FL:2-0;")]
    [InlineData("FL:1-0;", "FL:2-0;", "FL:2-0;")]
    [InlineData(null, "", "")]
    public void AppendFullTimeBetStatus_BetStatus_ReturnsExpectedNewBetStatus(string betStatus, string fullTimeBetStatus, string expectedNewBetStatus)
    {
        // Act
        var betStatusService = Substitute.For<BetStatusService>();
        var service = new BetStatusHalftimeServiceImp(betStatusService);
        var actualBetStatus = service.AppendFullTimeBetStatus(betStatus, fullTimeBetStatus);

        // Assert
        Assert.Equal(expectedNewBetStatus, actualBetStatus);
    }
}