using Fanex.Data.Repository;

namespace Fanex.Settlement.ScaleOut.Criteria
{
    public class ValidateEventCriteria : NonQueryCommand
    {
        public int MatchId { get; set; }

        public string Action { get; set; }

        public string Mode { get; set; }

        public bool IsAuto { get; set; } = false;

        public string SelectedResources { get; set; }

        public string SpName { get; set; }

        public int SDTRecord { get; set; }

        public int SettleAbandonedMatchIBCParlay { get; set; }

        public bool IsSettleDetermined { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override string GetSettingKey() => "NAP_Admin_Settlement_Validation_Event";

        public override bool IsValid() => MatchId > 0;
    }
}
