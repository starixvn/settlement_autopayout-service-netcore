using Fanex.Data.Repository;

namespace AutoPayout.Sportsbook.Criteria
{
    public class MatchListingCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "NAP_Set_AutoPayout_ScheduleSel";
        }
    }
}
