namespace AutoPayout.Sportsbook.Services;

using Constants;
using Exceptions;
using Models;

public interface ValidationGoalRecordService
{
    void VerifyGoalRecord(Match match);
}

public class ValidationGoalRecordServiceImp(ScoreAndGoalRecordService scoreAndGoalRecordService)
    : ValidationGoalRecordService
{
    public void VerifyGoalRecord(Match match)
    {
        if (IsUnCheckgoalRecord(match))
        {
            return;
        }

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

            return;
        }

        if (!scoreAndGoalRecordService.IsConsistentHalftime(match))
        {
            throw new GoalRecordHalftimeScoreNotConsistentException();
        }

        if (!scoreAndGoalRecordService.IsConsistentFullTime(match))
        {
            throw new GoalRecordFinalScoreNotConsistentException();
        }
    }

    private static bool IsUnCheckgoalRecord(Match match)
    {
        return match.IsSabaPenaltyShootout
            || match.IsNthCorner
            || match.MatchType == MatchTypes.ExtraTime
            || match.MatchType == MatchTypes.TotalGoalsMinutes;
    }
}