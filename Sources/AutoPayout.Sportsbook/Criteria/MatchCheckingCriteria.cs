using Fanex.Data;
using Fanex.Data.Repository;

namespace AutoPayout.Sportsbook.Criteria
{
    public class MatchCheckingCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        [SpParam(Ignored = true)]
        public string DbServer { get; set; }

        public override bool IsValid() => MatchId > 0;

        public override string GetSettingKey() => $"NAP_Set_AutoPayout_MatchChecking_{DbServer}";
    }
}