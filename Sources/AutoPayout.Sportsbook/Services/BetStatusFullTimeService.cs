using Kaden.Extensions;

namespace AutoPayout.Sportsbook.Services
{
    public interface BetStatusFullTimeService
    {
        string BetStatus(string currentBetStatus, string goalRecord);
    }

    public class BetStatusFullTimeServiceImp : BetStatusFullTimeService
    {
        private const string HtFirstLastGoalSymbol = "1FL";

        private const string FtFirstLastGoalSymbol = "FL";

        private const int MaxLengthWithHalftimeFgLg = 8;

        private readonly BetStatusService betStatusService;

        public BetStatusFullTimeServiceImp(BetStatusService betStatusService)
        {
            this.betStatusService = betStatusService;
        }

        public string BetStatus(string currentBetStatus, string goalRecord)
        {
            var fullTimeGoalRecord = goalRecord.Replace(";", string.Empty);
            var fullTimeBetStatus = betStatusService.GenerateBetStatus(FtFirstLastGoalSymbol, fullTimeGoalRecord);

            return ReplaceFullTimeFgLgOnBetStatus(currentBetStatus, fullTimeBetStatus);
        }

        private static string ReplaceFullTimeFgLgOnBetStatus(string currentBetStatus, string fullTimeBetStatus)
        {
            var maxLength = 7;
            var findSymbol = string.Empty;
            if (currentBetStatus.IgnoreCaseContains(HtFirstLastGoalSymbol))
            {
                maxLength = MaxLengthWithHalftimeFgLg;
                findSymbol = ";";
            }

            var indexFullTime = currentBetStatus.IgnoreCaseIndexOf($"{findSymbol}{FtFirstLastGoalSymbol}");
            if (indexFullTime > -1)
            {
                var fullTimeValue = currentBetStatus.Substring(indexFullTime, maxLength);
                return currentBetStatus.Replace(fullTimeValue, $"{findSymbol}{fullTimeBetStatus}");
            }

            return $"{currentBetStatus}{fullTimeBetStatus}";
        }
    }
}