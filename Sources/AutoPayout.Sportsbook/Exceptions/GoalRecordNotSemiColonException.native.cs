namespace AutoPayout.Sportsbook.Exceptions;

[Serializable]
public class GoalRecordNotSemiColonException : AutoPayoutException
{
    public GoalRecordNotSemiColonException()
    {
        Description = "Failed: Goal Record must contain \";\" character.";
    }

    public GoalRecordNotSemiColonException(string message)
        : base(message)
    {
    }

    public GoalRecordNotSemiColonException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    [Obsolete(DiagnosticId = "SYSLIB0051")]
    protected GoalRecordNotSemiColonException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}