using AutoPayout.Core.Models;
using DirectPayout.Early.Criteria;
using DirectPayout.Early.Models;
using Fanex.Settlement.ScaleOut.Services;
using Microsoft.Extensions.Options;

namespace DirectPayout.Early.Services;

public interface MatchSettleService
{
    Task<Result> Settle(Match match, int sourceId);
}

public class MatchSettleServiceImp : MatchSettleService
{
    private readonly ScaleOutSettleService scaleOutSettleService;
    private readonly IDynamicRepository repository;
    private readonly AppSettingsKey appSettingsKey;

    public MatchSettleServiceImp(ScaleOutSettleService scaleOutSettleService, IDynamicRepository repository, IOptions<AppSettingsKey> options)
    {
        this.scaleOutSettleService = scaleOutSettleService;
        this.repository = repository;
        this.appSettingsKey = options.Value;
    }



    //public MatchSettleServiceImp(ScaleOutSettleService scaleOutSettleService)
    //{
    //    this.scaleOutSettleService = scaleOutSettleService;
    //}

    public async Task<Result> Settle(Match match, int sourceId)
    {
        var resultValidate = await ValidateBeforeExecute();
        if (resultValidate.ErrCode != 0)
        {
            return resultValidate;
        }

        await scaleOutSettleService.UpdateUnderSettlementFlag(match.MatchId, 0, "");

        await ExcuteAllSite(new SettleCriteria
        {
            MatchId = match.MatchId,
        });

        return new Result();
    }

    //private async Task WriteLog(Match match, InsertLogCriteria insertLogCriteria, string actionName)
    //{
    //    await logService.WriteLogResettle(insertLogCriteria);

    //    await logService.WriteLog(match, actionName);
    //}

    private async Task<Result> ExcuteOnSite(SettleCriteria criteria, string server)
    {
        criteria.DbServer = server;

        await repository.ExecuteAsync(criteria);

        return new Result
        {
            ErrCode = criteria.ErrCode,
            ErrMsg = criteria.ErrMsg
        };
    }

    private async Task<Result> ExcuteAllSite(SettleCriteria criteria)
    {
        var servers = appSettingsKey.SiteDbServer;
        var tasks = servers.Select(x => ExcuteOnSite(criteria, x)).ToList();
        var results = await Task.WhenAll(tasks);

        return results.First(x => x.ErrCode != 0) ?? new Result();
    }


    private async Task<Result> ValidateBeforeExecute()
    {
        //var request = new EventTicketRequest
        //{
        //    MatchId = criteria.MatchId,
        //    Action = criteria.Action,
        //    //SpName = criteria.SpName,
        //    //Mode = Action.Mode.ContainsKey(criteria.Action) ? Action.Mode[criteria.Action] : "ALL",
        //    SelectedResources = criteria.ResourceResult
        //};

        //var result = await scaleOutSettleService.ValidateEventAndTicket(request);


        //return new Result
        //{
        //    ErrCode = result.ErrorCode,
        //    ErrMsg = result.ErrorMessage
        //};
        return new Result();
    }
}