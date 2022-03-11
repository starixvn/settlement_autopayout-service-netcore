using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class BetStatusFullTimeServiceTests
    {
        [Theory]
        [InlineData("1FL:1-0;", "1FL:1-0;FL:1-2;")]
        [InlineData("1FL:1-0;FL:1-0;", "1FL:1-0;FL:1-2;")]
        [InlineData("", "FL:1-2;")]
        [InlineData("Refund;", "Refund;FL:1-2;")]
        [InlineData("Refund;FL:1-0;", "Refund;FL:1-2;")]
        public void BetStatus_WithCurrentBetStatusAndNewFullTimeFgLg_ReturnsRightExpectedBetStatus(string currentBetStatus, string expectedBetStatus)
        {
            // Arrange
            var betStatusService = Substitute.For<BetStatusService>();
            var goalRecord = string.Empty;
            var prefix = "FL";
            betStatusService.GenerateBetStatus(prefix, goalRecord).Returns("FL:1-2;");

            var service = new BetStatusFullTimeServiceImp(betStatusService);
            // Act
            var actual = service.BetStatus(currentBetStatus, goalRecord);

            // Assert
            Assert.Equal(expectedBetStatus, actual);
        }
    }
}