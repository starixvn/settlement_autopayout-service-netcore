using AutoPayout.Sportsbook.Constants;

namespace AutoPayout.Sportsbook.Exceptions;

[Serializable]
public class RelatedMatchesInconsistentException : AutoPayoutException
{
    public RelatedMatchesInconsistentException(int typeId, string mainScore, string cornerScore)
    {
        var type = typeId == SettleType.Halftime ? "HT" : "FT";
        Description = $"Failed: Cannot proceed due to {type} value from current, {mainScore}, and {cornerScore} are inconsistent.";
    }

    public RelatedMatchesInconsistentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    [Obsolete(DiagnosticId = "SYSLIB0051")]
    protected RelatedMatchesInconsistentException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}