using Fanex.Data.Repository;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AutoPayout.Sportsbook.Services
{
    public interface ScoreService
    {
        Task<Score> TryGetScore(Match match);

        Task<List<ResultDetail>> TryGetRelatedResults(Match match);
    }

    public class ScoreServiceImp : ScoreService
    {
        private readonly IDynamicRepository repository;

        private readonly AppSettingsKey appSettingsKey;

        private readonly MatchQueryService matchQueryService;

        public ScoreServiceImp(
            IDynamicRepository repository,
            IOptions<AppSettingsKey> appSettingsKey,
            MatchQueryService matchQueryService)
        {
            this.repository = repository;
            this.appSettingsKey = appSettingsKey.Value;
            this.matchQueryService = matchQueryService;
        }

        public async Task<Score> TryGetScore(Match match)
        {
            for (var i = 0; i < appSettingsKey.RetryCount; i++)
            {
                var score = await GetScore(match);
                if (score != null)
                {
                    return score;
                }

                if (i < appSettingsKey.RetryCount - 1)
                {
                    await Task.Delay(appSettingsKey.RetryInterval);
                }
            }

            return new Score();
        }

        private async Task<Score> GetScore(Match match)
        {
            var criteria = new MatchScoreCriteria { MatchId = match.MatchId };

            return (await repository.FetchAsync<Score>(criteria))
                             .Where(s => s.SourceId == Source.Sbo)
                             .FirstOrDefault(s => s.TypeId == match.TypeId);
        }

        public async Task<List<ResultDetail>> TryGetRelatedResults(Match match)
        {
            for (var i = 0; i < appSettingsKey.TotalGoalCornerRetryCount; i++)
            {
                var resultsChecking = await GetResultDetail(match);
                if (IsResultsCalculated(resultsChecking))
                {
                    return resultsChecking;
                }

                if (i < appSettingsKey.RetryCount - 1)
                {
                    await Task.Delay(appSettingsKey.TotalGoalCornerInterval);
                }
            }

            return new List<ResultDetail>();
        }

        private async Task<List<ResultDetail>> GetResultDetail(Match match)
        {
            var resourceId = match.TypeId == SettleType.Halftime ? ResourceId.HalfTime : ResourceId.FullTime;
            var (_, resultDetails) = await matchQueryService.GetMatchAndResultDetail(match.MatchId);
            return resultDetails.Where(result => result.ResourceId == resourceId).ToList();
        }

        private static bool IsResultsCalculated(IEnumerable<ResultDetail> resultDetails)
        {
            return resultDetails.All(resultDetail => resultDetail.SettlementStatusId == SettlementStatus.Calculated);
        }
    }
}