using System;
using System.Runtime.Serialization;

namespace AutoPayout.Sportsbook.Exceptions
{
    [Serializable]
    public class RelatedMatchesNotCalculatedException : AutoPayoutException
    {
        public RelatedMatchesNotCalculatedException(string remark)
        {
            const string message = "Failed: Cannot proceed due to {0} from main or corner match has not been calculated.";
            Description = string.Format(message, remark);
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
}
