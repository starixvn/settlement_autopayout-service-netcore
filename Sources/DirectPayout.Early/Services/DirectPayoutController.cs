namespace DirectPayout.Early.Services;

public interface DirectPayoutController
{
    Task ProcessAsync();
}

public class DirectPayoutControllerImp(
    MatchSettlingPoolService matchSettlingPoolService,
    MatchSettleController matchSettleController)
    : DirectPayoutController
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