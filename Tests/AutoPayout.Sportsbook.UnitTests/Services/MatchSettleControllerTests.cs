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
        public void Process_ScoreMatchedAndSettleSuccess_InvokeAllRelatedMethods()
        {
            // Arrange
            var anyMatch = new Match { MatchId = 1 };
            var scoreMatcherController = Substitute.For<ScoreMatcherController>();
            var matchSettleService = Substitute.For<MatchSettleService>();
            matchSettleService.Settle(anyMatch, Arg.Any<int>()).Returns(new Models.Result());
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
           controller.Process(anyMatch);

            Received.InOrder(() =>
            {
                matchListingService.Received().Remove(anyMatch);
            });
        }

        [Fact]
        public void Process_ScoreMatchedButSettleUnSuccess_InvokeAllRelatedMethods()
        {
            // Arrange
            var anyMatch = new Match { MatchId = 1 };
            var scoreMatcherController = Substitute.For<ScoreMatcherController>();
            var matchSettleService = Substitute.For<MatchSettleService>();
            matchSettleService.Settle(anyMatch, Arg.Any<int>()).Returns(new Models.Result { ErrMsg = "Error message"});
            var settleLogService = Substitute.For<SettleLogService>();
            var matchListingService = Substitute.For<MatchSettlingPoolService>();
            var autoPayoutResultService = Substitute.For<AutoPayoutResultService>();
            var controller = new MatchSettleControllerImp(matchSettleService, scoreMatcherController, settleLogService, matchListingService, autoPayoutResultService);

            // Assert
            controller.Process(anyMatch);

            Received.InOrder(() =>
            {
                settleLogService.Received().Log(anyMatch, Arg.Any<string>());
                autoPayoutResultService.Received().Insert(anyMatch, Arg.Any<int>(), Arg.Any<int>());
                matchListingService.Received().Remove(anyMatch);
            });
        }

        [Fact]
        public void Process_ThrowExceptionOnVerifyMatch_InvokeAllRelatedMethods()
        {
            // Arrange
            var anyMatch = new Match { MatchId = 1 };
            var scoreMatcherController = Substitute.For<ScoreMatcherController>();
            var matchSettleService = Substitute.For<MatchSettleService>();
            matchSettleService.Settle(anyMatch, Arg.Any<int>()).Returns(new Models.Result { ErrCode = 1 });
            var settleLogService = Substitute.For<SettleLogService>();
            var matchListingService = Substitute.For<MatchSettlingPoolService>();
            var autoPayoutResultService = Substitute.For<AutoPayoutResultService>();
            var controller = new MatchSettleControllerImp(matchSettleService, scoreMatcherController, settleLogService, matchListingService, autoPayoutResultService);
            scoreMatcherController.Verify(anyMatch).Returns(Task.FromException<int>(new AutoPayoutException()));

            // Assert
            controller.Process(anyMatch);

            Received.InOrder(() =>
            {
                settleLogService.Received().Log(anyMatch, Arg.Any<AutoPayoutException>());
                autoPayoutResultService.Received().Insert(anyMatch, 0, MatchStatus.Failed);
                matchListingService.Received().Remove(anyMatch);
            });
        }
    }
}