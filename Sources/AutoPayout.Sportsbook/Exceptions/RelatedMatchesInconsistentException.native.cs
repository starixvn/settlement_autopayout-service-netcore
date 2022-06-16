using System;
using System.Runtime.Serialization;

namespace AutoPayout.Sportsbook.Exceptions
{
    [Serializable]
    public class RelatedMatchesInconsistentException : AutoPayoutException
    {
        public RelatedMatchesInconsistentException(string type, string mainScore, string cornerScore)
        {
            const string message = "Failed: Cannot proceed due to {0} value from current, {1}, and {2} are inconsistent.";
            Description = string.Format(message, type, mainScore, cornerScore);
        }

        public RelatedMatchesInconsistentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RelatedMatchesInconsistentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
