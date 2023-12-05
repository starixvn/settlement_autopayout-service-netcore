namespace AutoPayout.Sportsbook.Models;

using Constants;

public class Match
{
    private string goalRecord;

    private const int ToBase = 2;

    private const int DirectSettlement = 4;

    private const int FastMarketSettlement = 2;

    public int MatchId { get; set; }

    public int LeagueId { get; set; }

    public int? HtHomeScore { get; set; }

    public int? HtAwayScore { get; set; }

    public int? FinalHomeScore { get; set; }

    public int? FinalAwayScore { get; set; }

    public int TypeId { get; set; }

    public string BetStatus { get; set; }

    public int AutoSetting { get; set; }

    public bool IsDirectSettlement
    {
        get
        {
            var value = Convert.ToString(AutoSetting, ToBase).Reverse();
            return !string.IsNullOrWhiteSpace(value) && value.Length > DirectSettlement && value[DirectSettlement].Equals('1');
        }
    }

    public bool IsAutoFastMarketSettlement
    {
        get
        {
            var value = Convert.ToString(AutoSetting, ToBase).Reverse();
            return !string.IsNullOrWhiteSpace(value) && value.Length > FastMarketSettlement && value[FastMarketSettlement].Equals('1');
        }
    }

    public string GoalRecord
    {
        get =>
            string.IsNullOrWhiteSpace(goalRecord)
                ? string.Empty
                : goalRecord.ToUpperInvariant();

        set => goalRecord = value;
    }

    public byte SettlementStatus { get; set; }

    public RemarkSettingModel Remark => new RemarkSettingModel
    {
        SettlementStatus = Convert.ToString(SettlementStatus, ToBase).Reverse(),
    };

    public bool IsHalfTimeConfirmed { get; set; }

    public bool IsFullTimeConfirmed { get; set; }

    public bool IsHalfTimeCalculated { get; set; }

    public int LeagueGroupId { get; set; }

    public int ChildMatchType { get; set; }

    public int MainType { get; set; }

    public int ChildLeagueType { get; set; }

    public bool IsSabaPenaltyShootout => SabaSoccer.LeagueGroupIds.Contains(LeagueGroupId) && SabaSoccer.LeagueIds.Contains(LeagueId)
        && ChildMatchType is MatchType.PenaltyShootoutHdp or MatchType.PenaltyShootoutOu;

    public bool IsTotalCornerGoalMatch => MainType == MatchType.TotalCornerMainLeague && ChildLeagueType == MatchType.TotalCornerChildLeague;

    public override bool Equals(object obj)
    {
        return obj is Match match
               && match.MatchId == MatchId
               && match.TypeId == TypeId;
    }

    public override int GetHashCode() => MatchId * TypeId;
}