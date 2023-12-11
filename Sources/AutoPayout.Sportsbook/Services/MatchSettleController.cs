namespace AutoPayout.Sportsbook.Services;

using Exceptions;
using Models;

public interface MatchSettleController
{
    Task Process(Match match);
}

public class MatchSettleControllerImp : MatchSettleController
{
    private readonly MatchSettleService matchSettleService;

    private readonly ScoreMatcherController scoreMatcherController;

    private readonly SettleLogService settleLogService;

    private readonly MatchSettlingPoolService matchSettlingPoolService;

    private readonly InsertInfoService insertInfoService;

    public MatchSettleControllerImp(
        MatchSettleService matchSettleService,
        ScoreMatcherController scoreMatcherController,
        SettleLogService settleLogService,
        MatchSettlingPoolService matchSettlingPoolService,
        InsertInfoService insertInfoService)
    {
        this.matchSettleService = matchSettleService;
        this.scoreMatcherController = scoreMatcherController;
        this.settleLogService = settleLogService;
        this.matchSettlingPoolService = matchSettlingPoolService;
        this.insertInfoService = insertInfoService;
    }

    public async Task Process(Match match)
    {
        try
        {
            var sourceId = await scoreMatcherController.Verify(match);
            var result = await matchSettleService.Settle(match, sourceId);
            if (!string.IsNullOrWhiteSpace(result.ErrMsg))
            {
                await settleLogService.Log(match, result.ErrMsg);
                await insertInfoService.InsertResult(match, sourceId, MatchStatus.Failed);
            }
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
        await settleLogService.Log(match, exception);
        await insertInfoService.InsertResult(match, 0, MatchStatus.Failed);
    }
}