using AutoPayout.Core.Models;
using AutoPayout.Sportsbook.Constants;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using Fanex.Settlement.MessageBridge.JackTime;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using MassTransit;
using Microsoft.Extensions.Options;
using LogService = Fanex.Logging.LogService;
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
    IPublishEndpoint publishEndpoint,
    XmlConvertService xmlConvertService)
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

        await SendRequestJT(match);
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


    private async Task SendRequestJT(Match match)
    {
        var mainResource = match.ResultDetails.Where(r => r.ResourceId is ResourceId.FullTime or ResourceId.HalfTime).ToList();

        if (mainResource.Any())
        {
            try
            {
                var resources = new Resources<ResultDetail>
                {
                    ResourceItems = mainResource
                };

                var message = new SoccerResult
                {
                    MatchId = match.MatchId,
                    Action = "CAL",
                    SelectedResources = xmlConvertService.Serialize(resources)
                };

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await publishEndpoint.Publish(message, cts.Token);
            }
            catch (OperationCanceledException)
            {
                await LogService.ErrorAsync($"SendRequestJT timeout for MatchId: {match.MatchId}");
            }
            catch (Exception ex)
            {
                await LogService.ErrorAsync($"SendRequestJT failed for MatchId: {match.MatchId}, Error: {ex.Message}");
            }
        }
    }
}