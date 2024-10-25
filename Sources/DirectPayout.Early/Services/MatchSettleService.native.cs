using AutoPayout.Core.Models;
using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface MatchSettleService
{
    Task<Result> Settle(Match match, int sourceId);
}

public class MatchSettleServiceImp : MatchSettleService
{
    //private readonly ScaleOutSettleService scaleOutSettleService;

    //public MatchSettleServiceImp(ScaleOutSettleService scaleOutSettleService)
    //{
    //    this.scaleOutSettleService = scaleOutSettleService;
    //}

    public async Task<Result> Settle(Match match, int sourceId)
    {
        return new Result();
    }

    //private async Task WriteLog(Match match, InsertLogCriteria insertLogCriteria, string actionName)
    //{
    //    await logService.WriteLogResettle(insertLogCriteria);

    //    await logService.WriteLog(match, actionName);
    //}


    //private async Task<Result> ValidateBeforeExecute(MatchSettleCriteria criteria)
    //{
    //    var request = new EventTicketRequest
    //    {
    //        MatchId = criteria.MatchId,
    //        Action = criteria.Action,
    //        //SpName = criteria.SpName,
    //        //Mode = Action.Mode.ContainsKey(criteria.Action) ? Action.Mode[criteria.Action] : "ALL",
    //        SelectedResources = criteria.ResourceResult
    //    };

    //    var resultMessage = await scaleOutSettleService.ValidateEventAndTicket(request);


    //    return new Result();
    //}
}