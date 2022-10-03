namespace AutoPayout.Sportsbook.Criteria;

public class MatchDetailCriteria : CriteriaBase
{
    public int MatchId { get; set; }

    public override bool IsValid() => MatchId > 0;

    public override string GetSettingKey() => "NAP_Set_AutoPayout_MatchSelDetail";
}