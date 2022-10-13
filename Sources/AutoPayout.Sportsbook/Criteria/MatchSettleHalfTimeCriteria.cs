using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Criteria;

public class MatchSettleHalfTimeCriteria : MatchSettleCriteria
{
    public MatchSettleHalfTimeCriteria(Match match, string betStatus) : base(match)
    {
        Action = "CAL-HT";
        HalfTimeHome = match.HtHomeScore;
        HalfTimeAway = match.HtAwayScore;
        BetStatus = betStatus;
    }

    public int? HalfTimeHome { get; set; }

    public int? HalfTimeAway { get; set; }
}