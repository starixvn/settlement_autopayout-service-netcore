namespace AutoPayout.Sportsbook.Exceptions;
[Serializable]
public class HoldResultException : AutoPayoutException
{
    public HoldResultException(string message)
        : base(message)
    {
        Description = $"Failed Auto Settle: {message} is on-hold.";
    }

    public HoldResultException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
