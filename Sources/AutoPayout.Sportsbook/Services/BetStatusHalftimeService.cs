using Kaden.Extensions;
using System.Collections.Generic;

namespace AutoPayout.Sportsbook.Services
{
    public interface BetStatusHalftimeService
    {
        string BetStatus(string currentBetStatus, string goalRecord);

        string FullTimeBetStatus(string goalRecord);

        string AppendFullTimeBetStatus(string betStatus, string fullTimeBetStatus);
    }

    public class BetStatusHalftimeServiceImp : BetStatusHalftimeService
    {
        private const string HtFirstLastGoalSymbol = "1FL";

        private readonly BetStatusService betStatusService;

        public BetStatusHalftimeServiceImp(BetStatusService betStatusService)
        {
            this.betStatusService = betStatusService;
        }

        public string BetStatus(string currentBetStatus, string goalRecord)
        {
            var halftimeGoalRecord = GetHalftimeGoalRecordOnly(goalRecord);
            var halftimeBetStatus = betStatusService.GenerateBetStatus(HtFirstLastGoalSymbol, halftimeGoalRecord);

            return currentBetStatus.IgnoreCaseContains(HtFirstLastGoalSymbol)
                ? ReplaceHalftimeBetStatus(currentBetStatus, halftimeBetStatus)
                : $"{halftimeBetStatus}{currentBetStatus}";
        }

        private static string GetHalftimeGoalRecordOnly(string goalRecord)
        {
            return goalRecord.Contains(";") ? goalRecord.Split(';')[0] : goalRecord;
        }

        private static string ReplaceHalftimeBetStatus(string currentBetStatus, string halfTimeBetStatus)
        {
            const int maxLengthHalfTime = 8;
            var betStatus = currentBetStatus.Substring(currentBetStatus.IgnoreCaseIndexOf(HtFirstLastGoalSymbol), maxLengthHalfTime);

            return $"{halfTimeBetStatus}{currentBetStatus.Replace(betStatus, string.Empty)}";
        }

        public string FullTimeBetStatus(string goalRecord)
        {
            var halftimeGoalRecord = GetHalftimeGoalRecordOnly(goalRecord);
            return betStatusService.GenerateBetStatus(halftimeGoalRecord);
        }

        public string AppendFullTimeBetStatus(string betStatus, string fullTimeBetStatus)
        {
            if (IsModifyFirstGoalLastGoal(betStatus, fullTimeBetStatus))
            {
                return fullTimeBetStatus;
            }

            var arr = betStatus?.Split(';');
            var newBetStatus = new List<string>();
            var flag = false;

            foreach (var item in arr)
            {
                switch (flag)
                {
                    case false when item.IgnoreCaseContains("FL") && !item.IgnoreCaseContains("1FL"):
                        flag = true;
                        newBetStatus.Add(fullTimeBetStatus.Replace(";", string.Empty));
                        break;
                    case false when IsModifyFirstGoalLastGoal(item, fullTimeBetStatus):
                        flag = true;
                        newBetStatus.Add(fullTimeBetStatus);
                        break;
                    default:
                        newBetStatus.Add(item);
                        break;
                }
            }

            return string.Join(";", newBetStatus);
        }

        private static bool IsModifyFirstGoalLastGoal(string item, string fullTimeBetStatus)
        {
            return string.IsNullOrWhiteSpace(item) && !string.IsNullOrWhiteSpace(fullTimeBetStatus);
        }
    }
}