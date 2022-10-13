namespace AutoPayout.Sportsbook.Services;

public interface AutoPayoutController
{
    Task ProcessAsync();
}

public class AutoPayoutControllerImp : AutoPayoutController
{
    private readonly MatchSettlingPoolService matchSettlingPoolService;

    private readonly MatchSettleController matchSettleController;

    public AutoPayoutControllerImp(MatchSettlingPoolService matchSettlingPoolService, MatchSettleController matchSettleController)
    {
        this.matchSettlingPoolService = matchSettlingPoolService;
        this.matchSettleController = matchSettleController;
    }

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