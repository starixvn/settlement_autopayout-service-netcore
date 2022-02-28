using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class BetStatusFullTimeServiceTests
    {
        private readonly BetStatusService betStatusService;

        private readonly BetStatusFullTimeService service;

        public BetStatusFullTimeServiceTests()
        {
            betStatusService = Substitute.For<BetStatusService>();
            service = new BetStatusFullTimeServiceImp(betStatusService);
        }

        [Theory]
        [InlineData("1FL:1-0;", "1FL:1-0;FL:1-2;")]
        [InlineData("1FL:1-0;FL:1-0;", "1FL:1-0;FL:1-2;")]
        [InlineData("", "FL:1-2;")]
        [InlineData("Refund;", "Refund;FL:1-2;")]
        [InlineData("Refund;FL:1-0;", "Refund;FL:1-2;")]
        public void BetStatus_WithCurrentBetStatusAndNewFullTimeFgLg_ReturnsRightExpectedBetStatus(string currentBetStatus, string expectedBetStatus)
        {
            // Arrange
            var goalRecord = string.Empty;
            var prefix = "FL";
            betStatusService.GenerateBetStatus(prefix, goalRecord).Returns("FL:1-2;");

            // Act
            var actual = service.BetStatus(currentBetStatus, goalRecord);

            // Assert
            Assert.Equal(expectedBetStatus, actual);
        }
    }
}