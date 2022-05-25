using System;
using AutoPayout.Sportsbook.Constants;
using Kaden.Extensions;
using System.Linq;

namespace AutoPayout.Sportsbook.Models
{
    public class Match
    {
        private string goalRecord;

        private const int ToBase = 2;

        private const int DirectSettlement = 4;

        public int MatchId { get; set; }

        public int LeagueId { get; set; }

        public int? HtHomeScore { get; set; }

        public int? HtAwayScore { get; set; }

        public int? FinalHomeScore { get; set; }

        public int? FinalAwayScore { get; set; }

        public int TypeId { get; set; }

        public string BetStatus { get; set; }

        public int AutoSetting { get; set; }

        public bool IsDirectSettlement
        {
            get
            {
                var value = Convert.ToString(AutoSetting, ToBase).Reverse();
                return !string.IsNullOrWhiteSpace(value) && value.Length > DirectSettlement && value[DirectSettlement].Equals('1');
            }
        }

        public string GoalRecord
        {
            get
            {
                return string.IsNullOrWhiteSpace(goalRecord)
                    ? string.Empty
                    : goalRecord.ToUpperInvariant();
            }
            set
            {
                goalRecord = value;
            }
        }

        public byte SettlementStatus { get; set; }

        public RemarkSettingModel Remark => new RemarkSettingModel
        {
            SettlementStatus = Convert.ToString(SettlementStatus, ToBase).Reverse(),
        };

        public bool IsHalfTimeConfirmed { get; set; }

        public bool IsFullTimeConfirmed { get; set; }

        public bool IsHalfTimeCalculated { get; set; }

        public int LeagueGroupId { get; set; }

        public int ChildMatchType { get; set; }

        public bool IsCalculateAllForPenaltyMatch => TypeId == SettleType.FullTime && IsSabaPenaltyShootout;

        private bool IsSabaPenaltyShootout => SabaSoccer.LeagueGroupIds.Contains(LeagueGroupId) && SabaSoccer.LeagueIds.Contains(LeagueId)
            && (ChildMatchType == SoccerChildMatchType.PenaltyShootoutHDP || ChildMatchType == SoccerChildMatchType.PenaltyShootoutOU);

        public override bool Equals(object obj)
        {
            return obj != null
                && (obj is Match)
                && (obj as Match).MatchId == MatchId
                && (obj as Match).TypeId == TypeId;
        }

        public override int GetHashCode()
        {
            return MatchId * TypeId;
        }
    }
}