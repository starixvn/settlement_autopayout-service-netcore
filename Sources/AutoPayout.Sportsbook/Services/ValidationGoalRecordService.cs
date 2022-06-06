using AutoPayout.Sportsbook.Constants;
using AutoPayout.Sportsbook.Exceptions;
using AutoPayout.Sportsbook.Models;

namespace AutoPayout.Sportsbook.Services
{
    public interface ValidationGoalRecordService
    {
        void VerifyGoalRecord(Match match);
    }

    public class ValidationGoalRecordServiceImp : ValidationGoalRecordService
    {
        private readonly ScoreAndGoalRecordService scoreAndGoalRecordService;

        public ValidationGoalRecordServiceImp(ScoreAndGoalRecordService scoreAndGoalRecordService)
        {
            this.scoreAndGoalRecordService = scoreAndGoalRecordService;
        }

        public void VerifyGoalRecord(Match match)
        {
            if (match.TypeId == SettleType.Halftime)
            {
                if (!scoreAndGoalRecordService.IsExistedSemiColon(match.GoalRecord))
                {
                    throw new GoalRecordNotSemiColonException();
                }

                if (!scoreAndGoalRecordService.IsConsistentHalftime(match))
                {
                    throw new GoalRecordHalftimeScoreNotConsistentException();
                }
            }
            else
            {
                if (!match.IsSabaPenaltyShootout && !scoreAndGoalRecordService.IsConsistentHalftime(match))
                {
                    throw new GoalRecordHalftimeScoreNotConsistentException();
                }

                if (!scoreAndGoalRecordService.IsConsistentFullTime(match))
                {
                    throw new GoalRecordFinalScoreNotConsistentException();
                }
            }
        }
    }
}