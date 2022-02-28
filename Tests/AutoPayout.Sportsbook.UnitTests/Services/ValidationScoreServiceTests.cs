using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class ValidationScoreServiceTests
    {
        private readonly ValidationScoreService service;

        public ValidationScoreServiceTests()
        {
            service = new ValidationScoreServiceImp();
        }

        [Fact]
        public void VerifyScore_MatchIsZero_ThrowsSourceNotFoundException()
        {
            // Assert
            Assert.Throws<SourceNotFoundException>(() => service.VerifyScore(Arg.Any<Match>(), new Score()));
        }

        [Theory]
        [InlineData(SettleType.Halftime)]
        [InlineData(SettleType.FullTime)]
        public void VerifyScore_ScoreMismatched_ThrowsScoreMismatchedException(int settleType)
        {
            // Arrange
            var match = new Match
            {
                MatchId = 1,
                TypeId = settleType,
                HtHomeScore = 1,
                HtAwayScore = 1,
                FinalHomeScore = 1,
                FinalAwayScore = 1
            };

            var score = new Score
            {
                MatchId = 1,
                HomeValue = "0",
                AwayValue = "1"
            };

            // Assert
            Assert.Throws<ScoreMismatchedException>(() => service.VerifyScore(match, score));
        }

        [Theory]
        [InlineData(SettleType.Halftime)]
        [InlineData(SettleType.FullTime)]
        public void VerifyStatusMatch_ScoreMatched_DoesNotThrowException(int settleType)
        {
            // Arrange
            var match = new Match
            {
                MatchId = 1,
                TypeId = settleType,
                HtHomeScore = 0,
                HtAwayScore = 1,
                FinalHomeScore = 0,
                FinalAwayScore = 1
            };

            var score = new Score
            {
                MatchId = 1,
                HomeValue = "0",
                AwayValue = "1"
            };

            // Assert
            var exception = Record.Exception(() => service.VerifyScore(match, score));
            Assert.Null(exception);
        }
    }
}