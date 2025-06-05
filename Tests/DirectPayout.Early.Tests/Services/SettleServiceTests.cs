using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using DirectPayout.Early.Services;
using Fanex.Data.Repository;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using MassTransit;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace DirectPayout.Early.Tests.Services;

public class SettleServiceTests
{
    private readonly ScaleOutSettleService scaleOutSettleService;
    private readonly AutoPayoutResultService autoPayoutResultService;
    private readonly LogSettleService logSettleService;
    private readonly SettleServiceImp settleService;

    public SettleServiceTests()
    {
        var repository = Substitute.For<IDynamicRepository>();
        var appSettingsKey = Options.Create(new AppSettingsKey { SiteDbServer = ["Server1", "Server2"] });
        scaleOutSettleService = Substitute.For<ScaleOutSettleService>();
        autoPayoutResultService = Substitute.For<AutoPayoutResultService>();
        logSettleService = Substitute.For<LogSettleService>();
        var publishEndpoint1 = Substitute.For<IPublishEndpoint>();
        settleService = new SettleServiceImp(
            scaleOutSettleService,
            repository,
            appSettingsKey,
            autoPayoutResultService,
            logSettleService,
            publishEndpoint1);
    }

    [Fact]
    public async Task Settle_ShouldLogAndUpdate_WhenCalled()
    {
        // Arrange
        var match = new Match { MatchId = 1 };
        var settleCriteria = new SettleCriteria
        {
            MatchId = match.MatchId,
            Action = "CAL",
            AdminId = 123,
            SelectedResources = "<resources></resources>"
        };

        // Act
        await settleService.Settle(match, settleCriteria);

        // Assert
        await logSettleService.Received(1).WriteLogResettle(match);
        await logSettleService.Received(1).WriteLog(settleCriteria);
        await scaleOutSettleService.Received(1).UpdateUnderSettlementFlag(settleCriteria.MatchId, settleCriteria.AdminId, settleCriteria.Action);
        await autoPayoutResultService.Received(1).InsOrUpdAutoPayoutResult(Arg.Any<Match>(), AutoPayoutResultStatus.Waiting);
        await scaleOutSettleService.Received(1).UpdateMatchInfo(Arg.Is<UpdateMatchInfoRequest>(req =>
            req.Action == settleCriteria.Action &&
            req.IsAuto &&
            req.MatchId == settleCriteria.MatchId &&
            req.SelectedResources == settleCriteria.SelectedResources &&
            req.AdminId == settleCriteria.AdminId));
    }
}