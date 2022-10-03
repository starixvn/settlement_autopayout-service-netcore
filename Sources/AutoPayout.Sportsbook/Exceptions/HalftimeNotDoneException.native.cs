namespace AutoPayout.Sportsbook.Exceptions;

[Serializable]
public class HalftimeNotDoneException : AutoPayoutException
{
    public HalftimeNotDoneException()
    {
        Description = "Failed: Cannot execute FT auto-settle process due to HT process (auto/manual) has not done yet.";
    }

    public HalftimeNotDoneException(string message)
        : base(message)
    {
    }

    public HalftimeNotDoneException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected HalftimeNotDoneException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}