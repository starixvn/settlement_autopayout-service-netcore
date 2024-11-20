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

    public string Description
    {
        get;
        protected set;
    }
}