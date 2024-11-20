using DirectPayout.Early.Criteria;
using DirectPayout.Early.Exceptions;
using DirectPayout.Early.Models;
using DirectPayout.Early.Services;
using Fanex.Settlement.Converter.Services;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace DirectPayout.Early.Tests.Services;

public class MatchSettleControllerTests
{
    private readonly MatchSettlingPoolService matchSettlingPoolService;
    private readonly ValidationService validationService;
    private readonly XmlConvertService xmlConvertService;
    private readonly SettleService settleService;
    private readonly AutoPayoutResultService autoPayoutResultService;
    private readonly LogSettleService logSettleService;
    private readonly MatchSettleControllerImp controller;

    public MatchSettleControllerTests()
    {
        matchSettlingPoolService = Substitute.For<MatchSettlingPoolService>();
        validationService = Substitute.For<ValidationService>();
        xmlConvertService = Substitute.For<XmlConvertService>();
        settleService = Substitute.For<SettleService>();
        autoPayoutResultService = Substitute.For<AutoPayoutResultService>();
        logSettleService = Substitute.For<LogSettleService>();

        controller = new MatchSettleControllerImp(
            matchSettlingPoolService,
            validationService,
            xmlConvertService,
            settleService,
            autoPayoutResultService,
            logSettleService
        );
    }

    [Fact]
    public async Task Process_ShouldExecuteSettle_WhenValidationPasses()
    {
        // Arrange
        var match = new Match { MatchId = 1, ResultDetails = new List<ResultDetail>() };

        xmlConvertService.Serialize(Arg.Any<Resources<ResultDetail>>()).Returns("<xml>");

        // Act
        await controller.Process(match);

        // Assert
        await validationService.Received(1).VerifyNoRunningOdds(match);
        await validationService.Received(1).VerifyEventAndTicket(Arg.Any<SettleCriteria>());
        await settleService.Received(1).Settle(match, Arg.Any<SettleCriteria>());
        matchSettlingPoolService.Received(1).Remove(match);
    }

    [Fact]
    public async Task Process_ShouldHandleAutoPayoutException_WhenThrown()
    {
        // Arrange
        var match = new Match { MatchId = 1, ResultDetails = [] };
        var exception = new AppException("Auto payout error");

        validationService.VerifyEventAndTicket(Arg.Any<SettleCriteria>()).Throws(exception);

        // Act
        await controller.Process(match);

        // Assert
        await autoPayoutResultService.Received(1).UpdAutoPayoutFailResult(Arg.Any<Match>(), Arg.Any<string>());
        await logSettleService.Received(1).WriteLogErrorSettle(Arg.Any<Match>(), Arg.Any<string>());
        matchSettlingPoolService.Received(1).Remove(Arg.Any<Match>());
    }
}