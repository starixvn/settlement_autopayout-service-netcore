using System.Threading.Tasks;
using Fanex.Data.Repository;
using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services
{
    public interface AutoPayoutResultService
    {
        Task Insert(Match match, int source, int status);
    }

    public class AutoPayoutResultServiceImp : AutoPayoutResultService
    {
        private readonly IDynamicRepository repository;

        public AutoPayoutResultServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public async Task Insert(Match match, int source, int status)
        {
            var criteria = new AddAutoPayoutResultCriteria
            {
                MatchId = match.MatchId,
                TypeId = match.TypeId,
                SourceId = source,
                StatusId = status
            };

            if (match.TypeId == SettleType.Halftime)
            {
                criteria.HomeValue = match.HtHomeScore ?? 0;
                criteria.AwayValue = match.HtAwayScore ?? 0;
            }
            else
            {
                criteria.HomeValue = match.FinalHomeScore ?? 0;
                criteria.AwayValue = match.FinalAwayScore ?? 0;
            }

            await repository.ExecuteAsync(criteria);
        }
    }
}