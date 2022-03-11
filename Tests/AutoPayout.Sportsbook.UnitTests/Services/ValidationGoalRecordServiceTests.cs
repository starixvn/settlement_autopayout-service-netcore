using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class ValidationGoalRecordServiceTests
    {
        [Fact]
        public void VerifyGoalRecord_SettleHalftimeAndIsNotExistedSemiColon_ThrowsGoalRecordNotSemiColonException()
        {
            // Arrange
            var scoreAndGoalRecordService = Substitute.For<ScoreAndGoalRecordService>();
            
            var match = new Match
            {
                GoalRecord = "AH;",
                HtHomeScore = 2,
                HtAwayScore = 2,
                TypeId = SettleType.Halftime
            };

            scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(false);
            var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

            // Assert
            Assert.Throws<GoalRecordNotSemiColonException>(() => service.VerifyGoalRecord(match));
        }

        [Fact]
        public void VerifyGoalRecord_SettleHalftimeAndNotConsistentHalftime_ThrowsGoalRecordHalftimeScoreNotConsistentException()
        {
            // Arrange
            var scoreAndGoalRecordService = Substitute.For<ScoreAndGoalRecordService>();
            var match = new Match
            {
                GoalRecord = "AH;",
                HtHomeScore = 2,
                HtAwayScore = 2,
                TypeId = SettleType.Halftime
            };

            scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);
            scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);
            var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

            // Assert
            Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
        }

        [Fact]
        public void VerifyGoalRecord_SettleHalftimeAndIsConsistentHalftime_DoesNotThrowException()
        {
            // Arrange
            var scoreAndGoalRecordService = Substitute.For<ScoreAndGoalRecordService>();
            var match = new Match
            {
                GoalRecord = "AH;",
                HtHomeScore = 1,
                HtAwayScore = 1,
                TypeId = SettleType.Halftime
            };

            scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
            scoreAndGoalRecordService.IsExistedSemiColon(Arg.Any<string>()).Returns(true);
            var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

            // Assert
            var exception = Record.Exception(() => service.VerifyGoalRecord(match));
            Assert.Null(exception);
        }

        [Fact]
        public void VerifyGoalRecord_SettleFullTimeAndNotConsistentHalftime_ThrowsGoalRecordHalftimeScoreNotConsistentException()
        {
            // Arrange
            var scoreAndGoalRecordService = Substitute.For<ScoreAndGoalRecordService>();
            var match = new Match
            {
                GoalRecord = "AH;",
                HtHomeScore = 2,
                HtAwayScore = 2,
                TypeId = SettleType.FullTime
            };

            scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(false);
            var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

            // Assert
            Assert.Throws<GoalRecordHalftimeScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
        }

        [Fact]
        public void VerifyGoalRecord_SettleFullTimeAndNotConsistentFullTime_ThrowsGoalRecordHalftimeScoreNotConsistentException()
        {
            // Arrange
            var scoreAndGoalRecordService = Substitute.For<ScoreAndGoalRecordService>();
            var match = new Match
            {
                GoalRecord = "AH;",
                HtHomeScore = 2,
                HtAwayScore = 2,
                TypeId = SettleType.FullTime
            };

            scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
            scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(false);
            var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

            // Assert
            Assert.Throws<GoalRecordFinalScoreNotConsistentException>(() => service.VerifyGoalRecord(match));
        }

        [Fact]
        public void VerifyGoalRecord_SettleFullTimeAndIsConsistent_DoesNotThrowException()
        {
            // Arrange
            var scoreAndGoalRecordService = Substitute.For<ScoreAndGoalRecordService>();
            var match = new Match
            {
                GoalRecord = "AH;",
                HtHomeScore = 1,
                HtAwayScore = 1,
                TypeId = SettleType.FullTime
            };

            scoreAndGoalRecordService.IsConsistentHalftime(Arg.Any<Match>()).Returns(true);
            scoreAndGoalRecordService.IsConsistentFullTime(Arg.Any<Match>()).Returns(true);
            var service = new ValidationGoalRecordServiceImp(scoreAndGoalRecordService);

            // Assert
            var exception = Record.Exception(() => service.VerifyGoalRecord(match));
            Assert.Null(exception);
        }
    }
}