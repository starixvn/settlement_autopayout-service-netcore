using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Criteria;

public class MatchSettleAllCriteria : MatchSettleCriteria
{
    public MatchSettleAllCriteria(Match match, string betStatus) : base(match)
    {
        Action = "CAL";
        HalfTimeHome = 0;
        HalfTimeAway = 0;
        FullTimeHome = match.FinalHomeScore ?? 0;
        FullTimeAway = match.FinalAwayScore ?? 0;
        BetStatus = betStatus;
    }
}