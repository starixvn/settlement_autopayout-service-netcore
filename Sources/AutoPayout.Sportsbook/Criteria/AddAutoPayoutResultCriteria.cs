using Fanex.Data.Repository;

namespace AutoPayout.Sportsbook.Criteria
{
    public class AddAutoPayoutResultCriteria : NonQueryCommand
    {
        public int MatchId { get; set; }

        public int SourceId { get; set; }

        public int TypeId { get; set; }

        public int HomeValue { get; set; }

        public int AwayValue { get; set; }

        public int StatusId { get; set; }

        public override bool IsValid()
        {
            return MatchId > 0;
        }

        public override string GetSettingKey()
        {
            return "NAP_Set_AutoPayoutResultIns";
        }
    }
}