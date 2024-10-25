using AutoPayout.Sportsbook.Services;
using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface MatchSettleController
{
    Task Process(Match match);
}

public class MatchSettleControllerImp(
    MatchSettleService matchSettleService,
    //SettleLogService settleLogService,
    MatchSettlingPoolService matchSettlingPoolService,
    ScoreMatcherController scoreMatcherController)
    //InsertInfoService insertInfoService)
    : MatchSettleController
{
    public async Task Process(Match match)
    {
        try
        {
            //var sourceId = await scoreMatcherController.Verify(match);
            //var result = await matchSettleService.Settle(match, sourceId);
            //if (!string.IsNullOrWhiteSpace(result.ErrMsg))
            //{
            //    await settleLogService.Log(match, result.ErrMsg);
            //   // await insertInfoService.InsertResult(match, sourceId, MatchStatus.Failed, result.ErrMsg);
            //}
        }
        //catch (AutoPayoutException exception)
        //{
        //    await HandleAutoPayoutException(match, exception);
        //}
        finally
        {
            matchSettlingPoolService.Remove(match);
        }
    }

    //private async Task HandleAutoPayoutException(Match match, AutoPayoutException exception)
    //{
    //    await settleLogService.Log(match, exception);
    //    await insertInfoService.InsertResult(match, 0, MatchStatus.Failed, exception.Description);
    //}
}