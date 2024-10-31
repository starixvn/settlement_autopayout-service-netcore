using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using Microsoft.Extensions.Options;

namespace DirectPayout.Early.Services;

public interface SettleService
{
    Task Settle(Match match, SettleCriteria settleCriteria);
}

public class SettleServiceImp : SettleService
{
    private readonly ScaleOutSettleService scaleOutSettleService;
    private readonly IDynamicRepository repository;
    private readonly AppSettingsKey appSettings;
    private readonly AutoPayoutResultService autoPayoutResultService;
    private readonly LogSettleService logSettleService;

    public SettleServiceImp(
        ScaleOutSettleService scaleOutSettleService,
        IDynamicRepository repository,
        IOptions<AppSettingsKey> options,
        AutoPayoutResultService autoPayoutResultService,
        LogSettleService logSettleService)
    {
        this.scaleOutSettleService = scaleOutSettleService;
        this.repository = repository;
        appSettings = options.Value;
        this.autoPayoutResultService = autoPayoutResultService;
        this.logSettleService = logSettleService;
    }

    public async Task Settle(Match match, SettleCriteria settleCriteria)
    {
        await logSettleService.WriteLogResettle(match);

        await logSettleService.WriteLog(settleCriteria);

        await scaleOutSettleService.UpdateUnderSettlementFlag(settleCriteria.MatchId, settleCriteria.AdminId, settleCriteria.Action);

        await autoPayoutResultService.InsOrUpdAutoPayoutResult(settleCriteria, AutoPayoutResultStatus.Waiting);

        await UpdateMatchInfo(settleCriteria);

        var result = await ExecuteAllSite(settleCriteria);

        await autoPayoutResultService.InsOrUpdAutoPayoutResult(settleCriteria, AutoPayoutResultStatus.Completed);
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
        var servers = appSettings.SiteDbServer;
        var tasks = servers.Select(x => ExecuteOnSite(criteria, x)).ToList();
        var results = await Task.WhenAll(tasks);

        return results.FirstOrDefault(x => x.ErrCode != 0) ?? new Result();
    }
}