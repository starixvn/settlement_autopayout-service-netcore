using AutoPayout.Sportsbook.Exceptions;
using DirectPayout.Early.Models;

namespace DirectPayout.Early.Services;

public interface MatchSettleController
{
    Task Process(Match match);
}

public class MatchSettleControllerImp : MatchSettleController
{
    private readonly MatchSettlingPoolService matchSettlingPoolService;

    public MatchSettleControllerImp(MatchSettlingPoolService matchSettlingPoolService)
    {
        this.matchSettlingPoolService = matchSettlingPoolService;
    }

    public async Task Process(Match match)
    {
        try
        {
            //verify
            //settle
            //log
            //insert

        }
        catch (AutoPayoutException exception)
        {
            await HandleAutoPayoutException(match, exception);
        }
        finally
        {
            matchSettlingPoolService.Remove(match);
        }
    }

    private async Task HandleAutoPayoutException(Match match, AutoPayoutException exception)
    {
        //await settleLogService.Log(match, exception);
        //await insertInfoService.InsertResult(match, 0, MatchStatus.Failed, exception.Description);
    }
}