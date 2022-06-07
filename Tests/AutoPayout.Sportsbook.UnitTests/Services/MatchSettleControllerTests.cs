using System.Threading.Tasks;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using NSubstitute;
using AutoPayout.Sportsbook.Services;
using Xunit;

namespace AutoPayout.Sportsbook.UnitTests.Services
{
    public class MatchSettleControllerUnitTests
    {
        [Fact]
        public async Task Process_ScoreMatchedAndSettleSuccess_InvokeAllRelatedMethods()
        {
            // Arrange
            var anyMatch = new Match { MatchId = 1 };
            var scoreMatcherController = Substitute.For<ScoreMatcherController>();
            scoreMatcherController.Verify(Arg.Is<Match>(m => m != null)).Returns(1);
            var matchSettleService = Substitute.For<MatchSettleService>();
            matchSettleService.Settle(Arg.Is<Match>(m => m != null), Arg.Is<int>(i => i != 0)).Returns(new Models.Result());
            var settleLogService = Substitute.For<SettleLogService>();
            var matchListingService = Substitute.For<MatchSettlingPoolService>();
            var autoPayoutResultService = Substitute.For<AutoPayoutResultService>();
            var controller = new MatchSettleControllerImp(
                matchSettleService,
                scoreMatcherController,
                settleLogService,
                matchListingService,
                autoPayoutResultService);

            // Assert
            await controller.Process(anyMatch);

            matchListingService.Received().Remove(anyMatch);
        }

        [Fact]
        public async Task Process_ScoreMatchedButSettleUnSuccess_InvokeAllRelatedMethods()
        {
            // Arrange
            var anyMatch = new Match { MatchId = 1 };
            var scoreMatcherController = Substitute.For<ScoreMatcherController>();
            scoreMatcherController.Verify(Arg.Is<Match>(m => m != null)).Returns(1);
            var matchSettleService = Substitute.For<MatchSettleService>();
            matchSettleService.Settle(Arg.Is<Match>(m => m != null), Arg.Is<int>(i => i != 0)).Returns(new Models.Result { ErrMsg = "Error message"});
            var settleLogService = Substitute.For<SettleLogService>();
            var matchListingService = Substitute.For<MatchSettlingPoolService>();
            var autoPayoutResultService = Substitute.For<AutoPayoutResultService>();
            var controller = new MatchSettleControllerImp(matchSettleService, scoreMatcherController, settleLogService, matchListingService, autoPayoutResultService);

            // Assert
            await controller.Process(anyMatch);

            await settleLogService.Received().Log(anyMatch, Arg.Any<string>());
            await autoPayoutResultService.Received().Insert(anyMatch, Arg.Any<int>(), Arg.Any<int>());
            matchListingService.Received().Remove(anyMatch);
        }

        [Fact]
        public async Task Process_ThrowExceptionOnVerifyMatch_InvokeAllRelatedMethods()
        {
            // Arrange
            var anyMatch = new Match { MatchId = 1 };
            var scoreMatcherController = Substitute.For<ScoreMatcherController>();
            var matchSettleService = Substitute.For<MatchSettleService>();
            matchSettleService.Settle(Arg.Is<Match>(m => m != null), Arg.Is<int>(i => i != 0)).Returns(new Models.Result { ErrCode = 1 });
            var settleLogService = Substitute.For<SettleLogService>();
            var matchListingService = Substitute.For<MatchSettlingPoolService>();
            var autoPayoutResultService = Substitute.For<AutoPayoutResultService>();
            var controller = new MatchSettleControllerImp(matchSettleService, scoreMatcherController, settleLogService, matchListingService, autoPayoutResultService);
            scoreMatcherController.Verify(anyMatch).Returns(Task.FromException<int>(new AutoPayoutException()));

            // Assert
            await controller.Process(anyMatch);

            await settleLogService.Received().Log(anyMatch, Arg.Any<AutoPayoutException>());
            await autoPayoutResultService.Received().Insert(anyMatch, 0, MatchStatus.Failed);
            matchListingService.Received().Remove(anyMatch);
        }
    }
}