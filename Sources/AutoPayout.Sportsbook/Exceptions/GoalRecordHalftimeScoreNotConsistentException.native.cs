namespace AutoPayout.Sportsbook.Exceptions;

[Serializable]
public class GoalRecordHalftimeScoreNotConsistentException : AutoPayoutException
{
    public GoalRecordHalftimeScoreNotConsistentException()
    {
        Description = "Failed: Goal Record of HT is not consistent with HT Scores.";
    }

    public GoalRecordHalftimeScoreNotConsistentException(string message)
        : base(message)
    {
    }

    public GoalRecordHalftimeScoreNotConsistentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}