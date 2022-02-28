using Fanex.Data.Repository;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace AutoPayout.Sportsbook.Services
{
    public interface ScoreService
    {
        Task<Score> TryGetScore(Match match);
    }

    public class ScoreServiceImp : ScoreService
    {
        private readonly IDynamicRepository repository;

        private readonly AppSettingsKey appSettingsKey;

        public ScoreServiceImp(
            IDynamicRepository repository,
            IOptions<AppSettingsKey> appSettingsKey)
        {
            this.repository = repository;
            this.appSettingsKey = appSettingsKey.Value;
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
    }
}