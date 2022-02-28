using System;
using System.Runtime.Serialization;

namespace AutoPayout.Sportsbook.Exceptions
{
    [Serializable]
    public class SourceNotFoundException : AutoPayoutException
    {
        public SourceNotFoundException()
        {
            Description = "Failed: Cannot find that match on Info Site.";
        }

        public SourceNotFoundException(string message)
            : base(message)
        {
        }

        public SourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SourceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}