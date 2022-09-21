using Fanex.Data.Repository;

namespace AutoPayout.Sportsbook.Criteria
{
    public class MatchScoreCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "NAP_Set_AutoPayout_ResultDetailByMatchIdSel";
    }
}
