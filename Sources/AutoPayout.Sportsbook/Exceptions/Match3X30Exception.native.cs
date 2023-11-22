namespace AutoPayout.Sportsbook.Exceptions;

[Serializable]
public class Match3X30Exception : AutoPayoutException
{
    public Match3X30Exception()
    {
        Description = "Failed: Cannot execute FT auto-settle process due to 3x30 match.";
    }

    public Match3X30Exception(string message)
        : base(message)
    {
    }

    public Match3X30Exception(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    [Obsolete(DiagnosticId = "SYSLIB0051")]
    protected Match3X30Exception(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}