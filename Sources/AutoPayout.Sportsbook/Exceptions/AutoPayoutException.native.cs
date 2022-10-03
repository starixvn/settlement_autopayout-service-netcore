namespace AutoPayout.Sportsbook.Exceptions;

[Serializable]
public class AutoPayoutException : Exception
{
    public AutoPayoutException()
    {
    }

    public AutoPayoutException(string message)
        : base(message)
    {
    }

    public AutoPayoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected AutoPayoutException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    public string Description
    {
        get;
        protected set;
    }
}