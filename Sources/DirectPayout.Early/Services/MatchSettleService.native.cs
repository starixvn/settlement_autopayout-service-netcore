using AutoPayout.Core.Models;
using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface MatchSettleService
{
    Task<Result> Settle(Match match, int sourceId);
}

public class MatchSettleServiceImp(
    MatchQueryService matchQueryService
   )
    : MatchSettleService
{
    public async Task<Result> Settle(Match match, int sourceId)
    {
        return new Result();
    }

    //private async Task WriteLog(Match match, InsertLogCriteria insertLogCriteria, string actionName)
    //{
    //    await logService.WriteLogResettle(insertLogCriteria);

    //    await logService.WriteLog(match, actionName);
    //}
}