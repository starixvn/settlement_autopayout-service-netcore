using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using Fanex.Settlement.MessageBridge.RefundEvents;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using MassTransit.Transports;
using Microsoft.Extensions.Options;

namespace DirectPayout.Early.Services;

public interface SettleService
{
    Task Settle(Match match, SettleCriteria settleCriteria);
}

public class SettleServiceImp(
    ScaleOutSettleService scaleOutSettleService,
    IDynamicRepository repository,
    IOptions<AppSettingsKey> appSettings,
    AutoPayoutResultService autoPayoutResultService,
    LogSettleService logSettleService,
    PublishEndpoint publishEndpoint)
    : SettleService
{
    public async Task Settle(Match match, SettleCriteria settleCriteria)
    {
        await logSettleService.WriteLogResettle(match);

        await logSettleService.WriteLog(settleCriteria);

        await scaleOutSettleService.UpdateUnderSettlementFlag(settleCriteria.MatchId, settleCriteria.AdminId, settleCriteria.Action);

        await autoPayoutResultService.InsOrUpdAutoPayoutResult(match, AutoPayoutResultStatus.Waiting);

        await UpdateMatchInfo(settleCriteria);

        _ = await ExecuteAllSite(settleCriteria);

        await autoPayoutResultService.InsOrUpdAutoPayoutResult(match, AutoPayoutResultStatus.Completed);

        await publishEndpoint.Publish(new ResultsCalculated(
            match.MatchId,
            match.SportType,
            match.ChildMatchType,
            match.ResultDetails.Select(x => new Fanex.Settlement.MessageBridge.RefundEvents.ResourceItem
            {
                ResourceId = x.ResourceId,
                HomeValue = x.HomeValue,
                AwayValue = x.AwayValue,

            }).ToList()), new CancellationTokenSource(TimeSpan.FromSeconds(3)).Token);
    }

    private async Task UpdateMatchInfo(SettleCriteria criteria)
    {
        var updateMatchInfoRequest = new UpdateMatchInfoRequest
        {
            Action = criteria.Action,
            IsAuto = true,
            MatchId = criteria.MatchId,
            SelectedResources = criteria.SelectedResources,
            AdminId = criteria.AdminId,
        };

        await scaleOutSettleService.UpdateMatchInfo(updateMatchInfoRequest);
    }

    private async Task<Result> ExecuteOnSite(SettleCriteria criteria, string server)
    {
        criteria.DbServer = server;

        await repository.ExecuteAsync(criteria);

        return new Result(criteria.ErrCode, criteria.ErrMsg);
    }

    private async Task<Result> ExecuteAllSite(SettleCriteria criteria)
    {
        var servers = appSettings.Value.SiteDbServer;
        var tasks = servers.Select(x => ExecuteOnSite(criteria, x)).ToList();
        var results = await Task.WhenAll(tasks);

        var errorResult = results.ToList().Find(x => x.ErrCode != 0);

        return errorResult ?? new Result();
    }
}