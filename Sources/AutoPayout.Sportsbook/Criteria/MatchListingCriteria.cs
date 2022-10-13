namespace AutoPayout.Sportsbook.Criteria;

public class MatchListingCriteria : CriteriaBase
{
    public override bool IsValid() => true;

    public override string GetSettingKey() => "NAP_Set_AutoPayout_ScheduleSel";
}
