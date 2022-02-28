using System.Threading.Tasks;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Models;
using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class ScoreMatcherControllerTests
    {
        private ValidationSettingMatchService validationSettingMatchService;

        private ValidationStatusMatchService validationStatusMatchService;

        private ValidationScoreService validationScoreService;

        private readonly ScoreService scoreService;

        private ValidationGoalRecordService validationGoalRecordService;

        private MatchQueryService matchQueryService;

        private readonly ScoreMatcherController controller;

        public ScoreMatcherControllerTests()
        {
            validationStatusMatchService = Substitute.For<ValidationStatusMatchService>();
            validationScoreService = Substitute.For<ValidationScoreService>();
            validationSettingMatchService = Substitute.For<ValidationSettingMatchService>();
            scoreService = Substitute.For<ScoreService>();
            validationGoalRecordService = Substitute.For<ValidationGoalRecordService>();
            matchQueryService = Substitute.For<MatchQueryService>();
            controller = new ScoreMatcherControllerImp(
                validationSettingMatchService,
                validationStatusMatchService,
                validationScoreService,
                scoreService,
                validationGoalRecordService,
                matchQueryService);
        }

        [Fact]
        public async Task Verify_IsDirectSettlementMatch_ReturnsSourceIdIsAdminSite()
        {
            // Arrange
            var match = new Match { AutoSetting = 16 };

            // Act
            var actualSourceId = await controller.Verify(match);

            // Assert
            var expectedSourceId = Source.AdminSite;
            Assert.Equal(expectedSourceId, actualSourceId);
        }

        [Fact]
        public async Task Verify_ValidMatch_ReturnsSourceIdIsSbo()
        {
            // Arrange
            var match = new Match();

            scoreService.TryGetScore(Arg.Any<Match>()).Returns(new Score { SourceId = Source.Sbo });

            // Act
            var actualSourceId = await controller.Verify(match);

            // Assert
            var expectedSourceId = Source.Sbo;
            Assert.Equal(expectedSourceId, actualSourceId);
        }
    }
}