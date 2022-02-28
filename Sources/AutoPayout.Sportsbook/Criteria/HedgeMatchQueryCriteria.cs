using Fanex.Data.Repository;

namespace AutoPayout.Sportsbook.Criteria
{
    public class HedgeMatchQueryCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public override bool IsValid()
        {
            return MatchId > 0;
        }

        public override string GetSettingKey()
        {
            return "NAP_Set_MatchSelHedge";
        }
    }
}