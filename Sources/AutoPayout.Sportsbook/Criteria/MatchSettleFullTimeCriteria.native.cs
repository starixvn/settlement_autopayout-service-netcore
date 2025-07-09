using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Criteria;

public class MatchSettleFullTimeCriteria : MatchSettleCriteria
{
    public MatchSettleFullTimeCriteria(Match match, string betStatus) : base(match)
    {
        Action = "CAL-FT";
        HalfTimeHome = match.HtHomeScore;
        HalfTimeAway = match.HtAwayScore;
        FullTimeHome = match.FinalHomeScore ?? 0;
        FullTimeAway = match.FinalAwayScore ?? 0;
        BetStatus = betStatus;
    }
}