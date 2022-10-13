using AutoPayout.Sportsbook.Constants;

namespace AutoPayout.Sportsbook.Exceptions;

[Serializable]
public class RelatedMatchesNotCalculatedException : AutoPayoutException
{
    public RelatedMatchesNotCalculatedException(int typeId)
    {
        var type = typeId == SettleType.Halftime ? "HT" : "FT";
        Description = $"Failed: Cannot proceed due to {type} from main or corner match has not been calculated.";
    }

    public RelatedMatchesNotCalculatedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected RelatedMatchesNotCalculatedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
