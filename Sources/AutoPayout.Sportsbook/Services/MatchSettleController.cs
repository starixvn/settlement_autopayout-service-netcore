using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;
using System.Threading.Tasks;

namespace AutoPayout.Sportsbook.Services
{
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

        private readonly AutoPayoutResultService autoPayoutResultService;

        public MatchSettleControllerImp(
            MatchSettleService matchSettleService,
            ScoreMatcherController scoreMatcherController,
            SettleLogService settleLogService,
            MatchSettlingPoolService matchSettlingPoolService,
            AutoPayoutResultService autoPayoutResultService)
        {
            this.matchSettleService = matchSettleService;
            this.scoreMatcherController = scoreMatcherController;
            this.settleLogService = settleLogService;
            this.matchSettlingPoolService = matchSettlingPoolService;
            this.autoPayoutResultService = autoPayoutResultService;
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
                    await autoPayoutResultService.Insert(match, sourceId, MatchStatus.Failed);
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
            await autoPayoutResultService.Insert(match, 0, MatchStatus.Failed);
        }
    }
}