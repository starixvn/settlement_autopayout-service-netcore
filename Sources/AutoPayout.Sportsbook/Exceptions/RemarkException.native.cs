namespace AutoPayout.Sportsbook.Exceptions;

[Serializable]
public class RemarkException : AutoPayoutException
{
    public RemarkException()
    {
        Description = "Failed: Cannot execute auto-settle process due to match is on hold.";
    }

    public RemarkException(string message)
        : base(message)
    {
    }

    public RemarkException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    [Obsolete(DiagnosticId = "SYSLIB0051")]
    protected RemarkException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}