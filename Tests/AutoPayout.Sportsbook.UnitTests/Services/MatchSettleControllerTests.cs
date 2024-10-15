using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using AutoPayout.Sportsbook.Services;

namespace AutoPayout.Sportsbook.UnitTests.Services;

public class MatchSettleControllerUnitTests
{
    [Fact]
    public async Task Process_ScoreMatchedAndSettleSuccess_InvokeAllRelatedMethods()
    {
        // Arrange
        var anyMatch = new Match { MatchId = 1 };

        var scoreMatcherController = Substitute.For<ScoreMatcherController>();
        var settleLogService = Substitute.For<SettleLogService>();
        var matchListingService = Substitute.For<MatchSettlingPoolService>();
        var autoPayoutResultService = Substitute.For<InsertInfoService>();
        var matchSettleService = Substitute.For<MatchSettleService>();

        matchSettleService.Settle(Arg.Any<Match>(), Arg.Any<int>()).Returns(new Core.Models.Result());

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
        scoreMatcherController.Verify(Arg.Any<Match>()).Returns(1);
        var matchSettleService = Substitute.For<MatchSettleService>();
        matchSettleService.Settle(Arg.Any<Match>(), Arg.Any<int>()).Returns(new Core.Models.Result { ErrMsg = "Error message" });
        var settleLogService = Substitute.For<SettleLogService>();
        var matchListingService = Substitute.For<MatchSettlingPoolService>();
        var autoPayoutResultService = Substitute.For<InsertInfoService>();
        var controller = new MatchSettleControllerImp(matchSettleService, scoreMatcherController, settleLogService, matchListingService, autoPayoutResultService);

        // Assert
        await controller.Process(anyMatch);

        await settleLogService.Received().Log(anyMatch, Arg.Any<string>());
        await autoPayoutResultService.Received().InsertResult(anyMatch, Arg.Any<int>(), Arg.Any<int>(), "Error message");
        matchListingService.Received().Remove(anyMatch);
    }

    [Fact]
    public async Task Process_ThrowExceptionOnVerifyMatch_InvokeAllRelatedMethods()
    {
        // Arrange
        var anyMatch = new Match { MatchId = 1 };
        var scoreMatcherController = Substitute.For<ScoreMatcherController>();
        var matchSettleService = Substitute.For<MatchSettleService>();
        matchSettleService.Settle(Arg.Any<Match>(), Arg.Any<int>()).Returns(new Core.Models.Result { ErrCode = 1 });
        var settleLogService = Substitute.For<SettleLogService>();
        var matchListingService = Substitute.For<MatchSettlingPoolService>();
        var autoPayoutResultService = Substitute.For<InsertInfoService>();
        var controller = new MatchSettleControllerImp(matchSettleService, scoreMatcherController, settleLogService, matchListingService, autoPayoutResultService);
        scoreMatcherController.Verify(anyMatch).Returns(Task.FromException<int>(new Match3X30Exception()));

        // Assert
        await controller.Process(anyMatch);

        await settleLogService.Received().Log(anyMatch, Arg.Any<AutoPayoutException>());
        await autoPayoutResultService.Received().InsertResult(anyMatch, 0, MatchStatus.Failed, Arg.Is<string>(s => !string.IsNullOrEmpty(s)));
        matchListingService.Received().Remove(anyMatch);
    }
}