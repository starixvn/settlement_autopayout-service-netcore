namespace AutoPayout.Result.Services;

public interface GoalRecordService
{
    string HalftimeGoal(string goalRecord);

    string FullTimeGoal(string goalRecord);
}

public class GoalRecordServiceImp : GoalRecordService
{
    public string HalftimeGoal(string goalRecord)
    {
        return IsExistedGoalRecord(goalRecord)
           ? goalRecord.Split(';')[0]
           : goalRecord;
    }

    public string FullTimeGoal(string goalRecord)
    {
        return goalRecord.Replace(";", string.Empty);
    }

    private static bool IsExistedGoalRecord(string goalRecord)
    {
        return !string.IsNullOrWhiteSpace(goalRecord) && goalRecord.Contains(';');
    }
}