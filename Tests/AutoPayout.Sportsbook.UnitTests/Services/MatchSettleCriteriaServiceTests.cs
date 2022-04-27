using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class MatchSettleCriteriaServiceTests
    {
        private readonly BetStatusHalftimeService betStatusHalftimeService;
        private readonly BetStatusFullTimeService betStatusFullTimeService;

        public MatchSettleCriteriaServiceTests()
        {
            betStatusHalftimeService = Substitute.For<BetStatusHalftimeService>();
            betStatusFullTimeService = Substitute.For<BetStatusFullTimeService>();
        }

        [Fact]
        public void CreateSettleCriteria_MatchIsHalftime_ReturnsRightActionName()
        {
            // Arrange
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
            const string expectedBetStatus = "1FL:1-1;";
            var match = new Models.Match { TypeId = 1 };
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
            const string expectedBetStatus = "1FL:1-1;FL:x-0";
            var match = new Models.Match { TypeId = 1 };
            var matchChecking = new Models.MatchChecking { IsExistHtFgLg = true, IsExistFgNg = true};

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
            const string expectedBetStatus = "1FL:1-1;FL:1-1;";
            var match = new Models.Match { TypeId = 2 };
            var matchChecking = new Models.MatchChecking { IsExistFgLg = true };

            betStatusFullTimeService.BetStatus(Arg.Any<string>(), Arg.Any<string>()).Returns("1FL:1-1;FL:1-1;");
            var matchSettleCriteriaService = new MatchSettleCriteriaServiceImp(betStatusHalftimeService, betStatusFullTimeService);

            // Act
            var actual = matchSettleCriteriaService.CreateSettleCriteria(match, matchChecking);

            // Assert
            Assert.Equal(expectedBetStatus, actual.BetStatus);
        }
    }
}