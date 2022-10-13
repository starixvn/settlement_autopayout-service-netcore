namespace AutoPayout.Sportsbook.Exceptions;

[Serializable]
public class GoalRecordFinalScoreNotConsistentException : AutoPayoutException
{
    public GoalRecordFinalScoreNotConsistentException()
    {
        Description = "Failed: Goal Record of FT is not consistent with FT Scores.";
    }

    public GoalRecordFinalScoreNotConsistentException(string message)
        : base(message)
    {
    }

    public GoalRecordFinalScoreNotConsistentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected GoalRecordFinalScoreNotConsistentException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}