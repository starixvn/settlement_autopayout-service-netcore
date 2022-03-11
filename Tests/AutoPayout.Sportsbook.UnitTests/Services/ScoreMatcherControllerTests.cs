using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;
using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class ScoreMatcherControllerTests
    {
        [Fact]
        public void Verify_IsDirectSettlementMatch_ReturnsSourceIdIsAdminSite()
        {
            // Arrange
            var validationStatusMatchService = Substitute.For<ValidationStatusMatchService>();
            var validationScoreService = Substitute.For<ValidationScoreService>();
            var validationSettingMatchService = Substitute.For<ValidationSettingMatchService>();
            var scoreService = Substitute.For<ScoreService>();
            var validationGoalRecordService = Substitute.For<ValidationGoalRecordService>();
            var matchQueryService = Substitute.For<MatchQueryService>();
            var controller = new ScoreMatcherControllerImp(
                validationSettingMatchService,
                validationStatusMatchService,
                validationScoreService,
                scoreService,
                validationGoalRecordService,
                matchQueryService);

            var match = new Match { AutoSetting = 16 };

            // Act
            var actualSourceId =  controller.Verify(match);

            // Assert
            const int expectedSourceId = Source.AdminSite;
            Assert.Equal(expectedSourceId, actualSourceId.Result);
        }

        [Fact]
        public void Verify_ValidMatch_ReturnsSourceIdIsSbo()
        {
            // Arrange
            var validationStatusMatchService = Substitute.For<ValidationStatusMatchService>();
            var validationScoreService = Substitute.For<ValidationScoreService>();
            var validationSettingMatchService = Substitute.For<ValidationSettingMatchService>();
            var scoreService = Substitute.For<ScoreService>();
            var validationGoalRecordService = Substitute.For<ValidationGoalRecordService>();
            var matchQueryService = Substitute.For<MatchQueryService>();
            var controller = new ScoreMatcherControllerImp(
                validationSettingMatchService,
                validationStatusMatchService,
                validationScoreService,
                scoreService,
                validationGoalRecordService,
                matchQueryService);
            var match = new Match();

            scoreService.TryGetScore(Arg.Any<Match>()).Returns(new Score { SourceId = Source.Sbo });

            // Act
            var actualSourceId = controller.Verify(match);

            // Assert
            const int expectedSourceId = Source.Sbo;
            Assert.Equal(expectedSourceId, actualSourceId.Result);
        }
    }
}