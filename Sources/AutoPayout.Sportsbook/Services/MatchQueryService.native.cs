using Fanex.Data.Repository;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoPayout.Sportsbook.Services
{
    public interface MatchQueryService
    {
        Task<List<Match>> GetWaitingMatches();

        Task<MatchHedgeDetail> GetHedgeMatchId(int matchId);

        Task<MatchChecking> GetMatchInfo(int matchId);

        Task<RemarkSettingModel> GetMatchRemark(int matchId);

        Task<Models.Result> Execute(MatchSettleCriteria criteria);
    }

    public class MatchQueryServiceImp : MatchQueryService
    {
        private readonly IDynamicRepository repository;

        public MatchQueryServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public async Task<List<Match>> GetWaitingMatches()
        {
            var criteria = new MatchListingCriteria();
            return (await repository.FetchAsync<Match>(criteria)).ToList();
        }

        public async Task<MatchHedgeDetail> GetHedgeMatchId(int matchId)
        {
            var criteria = new HedgeMatchQueryCriteria { MatchId = matchId };
            var match = await repository.GetAsync<MatchHedgeDetail>(criteria);
            return match ?? new MatchHedgeDetail();
        }

        public async Task<MatchChecking> GetMatchInfo(int matchId)
        {
            var criteria = new MatchCheckingCriteria { MatchId = matchId };
            return await repository.GetAsync<MatchChecking>(criteria);
        }

        public async Task<RemarkSettingModel> GetMatchRemark(int matchId)
        {
            var criteria = new MatchRemarkCriteria { MatchId = matchId };
            var match = await repository.GetAsync<MatchRemark>(criteria);
            return match.Remark;
        }

        public async Task<Models.Result> Execute(MatchSettleCriteria criteria)
        {
            await repository.ExecuteAsync(criteria);

            return new Models.Result
            {
                ErrCode = criteria.ErrCode,
                ErrMsg = criteria.ErrMsg
            };
        }
    }
}