using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class BetStatusHalftimeServiceTests
    {
        private readonly BetStatusService betStatusService;

        private readonly BetStatusHalftimeService service;

        public BetStatusHalftimeServiceTests()
        {
            betStatusService = Substitute.For<BetStatusService>();
            service = new BetStatusHalftimeServiceImp(betStatusService);
        }

        [Theory]
        [InlineData("", "1FL:1-2;")]
        [InlineData("FL:2-0;", "1FL:1-2;FL:2-0;")]
        [InlineData("1FL:1-0;FL:1-0;", "1FL:1-2;FL:1-0;")]
        [InlineData("FL:1-0;1FL:1-0;", "1FL:1-2;FL:1-0;")]
        public void BetStatus_WithCurrentBetStatusAndNewHalftimeFgLg_ReturnsRightExpectedBetStatus(string currentBetStatus, string expectedBetStatus)
        {
            // Arrange
            betStatusService.GenerateBetStatus(Arg.Any<string>(), Arg.Any<string>()).Returns("1FL:1-2;");

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

        public void AppendFullTimeBetStatus_BetStatus_ReturnsExpectedNewBetStatus(string betStatus, string fullTimeBetStatus, string expectedNewBetStatus)
        {
            // Act
            var actualBetStatus = service.AppendFullTimeBetStatus(betStatus, fullTimeBetStatus);

            // Assert
            Assert.Equal(expectedNewBetStatus, actualBetStatus);
        }
    }
}