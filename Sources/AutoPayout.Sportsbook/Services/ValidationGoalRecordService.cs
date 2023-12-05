namespace AutoPayout.Sportsbook.Services;

using Constants;
using Exceptions;
using Models;

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

            if (!match.IsSabaPenaltyShootout && !scoreAndGoalRecordService.IsConsistentFullTime(match))
            {
                throw new GoalRecordFinalScoreNotConsistentException();
            }
        }
    }
}