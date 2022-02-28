using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Criteria;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services
{
    public interface MatchSettleCriteriaService
    {
        MatchSettleCriteria CreateSettleCriteria(Match match, MatchChecking matchChecking);
    }

    public class MatchSettleCriteriaServiceImp : MatchSettleCriteriaService
    {
        private readonly BetStatusHalftimeService betStatusHalftimeService;
        private readonly BetStatusFullTimeService betStatusFullTimeService;

        public MatchSettleCriteriaServiceImp(
            BetStatusHalftimeService betStatusHalftimeService,
            BetStatusFullTimeService betStatusFullTimeService)
        {
            this.betStatusHalftimeService = betStatusHalftimeService;
            this.betStatusFullTimeService = betStatusFullTimeService;
        }

        public MatchSettleCriteria CreateSettleCriteria(Match match, MatchChecking matchChecking)
        {
            if (match.TypeId == SettleType.Halftime)
            {
                var betStatus = BetStatusHalftime(match, matchChecking);
                return new MatchSettleHalfTimeCriteria(match, betStatus);
            }
            else
            {
                var betStatus = BetStatusFullTime(match, matchChecking);
                return new MatchSettleFullTimeCriteria(match, betStatus);
            }
        }

        private string BetStatusHalftime(Match match, MatchChecking matchChecking)
        {
            return matchChecking.IsExistHtFgLg ? betStatusHalftimeService.BetStatus(match.BetStatus, match.GoalRecord) : match.BetStatus;
        }

        private string BetStatusFullTime(Match match, MatchChecking matchChecking)
        {
            return matchChecking.IsExistFgLg ? betStatusFullTimeService.BetStatus(match.BetStatus, match.GoalRecord) : match.BetStatus;
        }
    }
}