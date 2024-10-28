using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Exceptions;
using Fanex.Settlement.Converter.Services;
using Fanex.Settlement.ScaleOut.Request;
using Fanex.Settlement.ScaleOut.Services;
using Microsoft.Extensions.Options;

namespace DirectPayout.Early.Services;

public interface SettleService
{
    Task Settle(SettleCriteria settleCriteria);
}

public class SettleServiceImp : SettleService
{
    private readonly ScaleOutSettleService scaleOutSettleService;
    private readonly IDynamicRepository repository;
    private readonly AppSettingsKey appSettings;
    private readonly AutoPayoutResultService autoPayoutResultService;

    public SettleServiceImp(
        ScaleOutSettleService scaleOutSettleService,
        IDynamicRepository repository,
        IOptions<AppSettingsKey> options,
        XmlConvertService xmlConvertService,
        ValidationService validationService,
        AutoPayoutResultService autoPayoutResultService)
    {
        this.scaleOutSettleService = scaleOutSettleService;
        this.repository = repository;
        appSettings = options.Value;
        this.autoPayoutResultService = autoPayoutResultService;
    }

    public async Task Settle(SettleCriteria settleCriteria)
    {
        await scaleOutSettleService.UpdateUnderSettlementFlag(settleCriteria.MatchId, settleCriteria.AdminId, settleCriteria.Action);
        await autoPayoutResultService.InsOrUpdAutoPayoutResult(settleCriteria, AutoPayoutResultStatus.Waiting);
        await UpdateMatchInfo(settleCriteria);
        var result = await ExecuteAllSite(settleCriteria);
        await autoPayoutResultService.InsOrUpdAutoPayoutResult(settleCriteria, AutoPayoutResultStatus.Completed);

        if (result.ErrCode != 0 && result.ErrCode != 99)
        {
            throw new AppException(result.ErrMsg);
        }
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
        updateMatchInfoRequest.IsAuto = true;

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

        return results.First(x => x.ErrCode != 0) ?? new Result();
    }
}