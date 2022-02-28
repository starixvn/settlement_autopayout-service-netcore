using AutoPayout.Sportsbook.Models;
using System.Linq;

namespace AutoPayout.Sportsbook.Services
{
    public interface ScoreAndGoalRecordService
    {
        bool IsConsistentHalftime(Match match);

        bool IsConsistentFullTime(Match match);

        bool IsExistedSemiColon(string goalRecord);
    }

    public class ScoreAndGoalRecordServiceImp : ScoreAndGoalRecordService
    {
        private const char Home = 'h';
        private const char Away = 'a';

        public bool IsConsistentHalftime(Match match)
        {
            var halftimeGoalRecord = match.GoalRecord.Split(';')[0];
            var home = match.HtHomeScore;
            var away = match.HtAwayScore;
            return IsConsistentScoreAndGoalRecord(home, away, halftimeGoalRecord);
        }

        public bool IsConsistentFullTime(Match match)
        {
            var fullTimeGoalRecord = match.GoalRecord.Replace(";", string.Empty);
            var home = match.FinalHomeScore;
            var away = match.FinalAwayScore;
            return IsConsistentScoreAndGoalRecord(home, away, fullTimeGoalRecord);
        }

        public bool IsExistedSemiColon(string goalRecord)
        {
            return !string.IsNullOrWhiteSpace(goalRecord) && goalRecord.IndexOf(';') >= 0;
        }

        private static bool IsConsistentScoreAndGoalRecord(int? home, int? away, string goalRecord)
        {
            if (home == null && away == null)
            {
                return false;
            }
            else
            {
                goalRecord = string.IsNullOrWhiteSpace(goalRecord)
                    ? string.Empty
                    : goalRecord.ToLowerInvariant();

                return home + away == goalRecord.Length
                       && home == goalRecord.Count(c => c == Home)
                       && away == goalRecord.Count(c => c == Away);
            }
        }
    }
}