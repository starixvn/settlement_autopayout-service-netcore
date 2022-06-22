using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;
using System.Collections.Generic;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class ValidationScoreServiceTests
    {
        private readonly ValidationScoreService service;

        public ValidationScoreServiceTests()
        {
            var otherInfoService = Substitute.For<OtherInfoService>();
            service = new ValidationScoreServiceImp(otherInfoService);
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

        [Fact]
        public void VerifyResultsConsistent_EmptyResultDetails_ThrowException()
        {
            var match = new Match();
            var resultDetails = new List<ResultDetail>();

            Assert.Throws<RelatedMatchesNotCalculatedException>(() => service.VerifyResultsConsistent(match, resultDetails));
        }

        [Fact]
        public void VerifyResultsConsistent_MatchAndResultsFullTimeNotConsistent_ThrowException()
        {
            var match = new Match
            {
                TypeId = 2,
                FinalHomeScore = 4,
                FinalAwayScore = 5
            };
            var resultDetails = new List<ResultDetail>();

            Assert.Throws<RelatedMatchesNotCalculatedException>(() => service.VerifyResultsConsistent(match, resultDetails));
        }

        [Fact]
        public void VerifyResultsConsistent_MatchAndResultsHalfTimeNotConsistent_ThrowException()
        {
            var match = new Match
            {
                TypeId = 1,
                HtHomeScore = 4,
                HtAwayScore = 5
            };
            var resultDetails = new List<ResultDetail>
            {
                new ResultDetail { ChildMatchType = 204, HomeValue = "2", AwayValue = "3" },
                new ResultDetail { HomeValue = "2", AwayValue = "4" }
            };

            Assert.Throws<RelatedMatchesInconsistentException>(() => service.VerifyResultsConsistent(match, resultDetails));
        }
    }
}