using Kaden.Extensions;

namespace AutoPayout.Sportsbook.Services
{
    public interface BetStatusHalftimeService
    {
        string BetStatus(string currentBetStatus, string goalRecord);
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
    }
}