namespace AutoPayout.Sportsbook.Services;

public interface AutoPayoutController
{
    Task ProcessAsync();
}

public class AutoPayoutControllerImp(
    MatchSettlingPoolService matchSettlingPoolService,
    MatchSettleController matchSettleController)
    : AutoPayoutController
{
    public async Task ProcessAsync()
    {
        var newMatches = await matchSettlingPoolService.PeekNewMatch();
        newMatches.ForEach(m =>
        {
            Task.Run(() => matchSettleController.Process(m))
                .ConfigureAwait(false);
        });
    }
}