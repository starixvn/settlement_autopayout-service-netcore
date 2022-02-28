using System;
using System.Runtime.Serialization;

namespace AutoPayout.Sportsbook.Exceptions
{
    [Serializable]
    public class ScoreMismatchedException : AutoPayoutException
    {
        public ScoreMismatchedException()
        {
            Description = "Failed: Suspender's score & Info Site's score are mismatched.";
        }

        public ScoreMismatchedException(string message)
            : base(message)
        {
        }

        public ScoreMismatchedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ScoreMismatchedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}