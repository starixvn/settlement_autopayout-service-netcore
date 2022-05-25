using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Criteria
{
    public class MatchSettleAllCriteria : MatchSettleCriteria
    {
        public MatchSettleAllCriteria(Match match, string betStatus) : base(match)
        {
            Action = "CAL";
            FullTimeHome = match.FinalHomeScore ?? 0;
            FullTimeAway = match.FinalAwayScore ?? 0;
            BetStatus = betStatus;
        }

        public int HalfTimeHome { get; }

        public int HalfTimeAway { get; }

        public int FullTimeHome { get; set; }

        public int FullTimeAway { get; set; }
    }
}
