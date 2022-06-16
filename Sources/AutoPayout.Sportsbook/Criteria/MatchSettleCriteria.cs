using AutoPayout.Sportsbook.Models;
using Fanex.Data.Repository;

namespace AutoPayout.Sportsbook.Criteria
{
    public class MatchSettleCriteria : NonQueryCommand
    {
        public MatchSettleCriteria(Match match)
        {
            AdminId = 0;
            ErrCode = 0;
            ErrMsg = string.Empty;
            MatchId = match.MatchId;
            GoalRecord = match.GoalRecord;
        }

        public int AdminId { get; set; }

        public int MatchId { get; set; }

        public string Action { get; set; }

        public string BetStatus { get; set; }

        public string GoalRecord { get; set; }

        public string ResourceResult { get; set; }

        public int SourceId { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string RefResult { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "NAP_Admin_Settlement_1_AutoPayout_Push";
        }
    }
}