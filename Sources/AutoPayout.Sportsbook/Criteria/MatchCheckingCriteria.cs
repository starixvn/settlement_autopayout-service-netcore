using Fanex.Data.Repository;

namespace AutoPayout.Sportsbook.Criteria
{
    public class MatchCheckingCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public override bool IsValid()
        {
            return MatchId > 0;
        }

        public override string GetSettingKey()
        {
            return "NAP_Set_AutoPayout_MatchChecking";
        }
    }
}