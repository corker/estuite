using System;
using System.Runtime.Serialization;

namespace Estuite
{
    [Serializable]
    public class EventStreamConcurrentWriteException : Exception
    {
        public EventStreamConcurrentWriteException()
        {
        }

        public EventStreamConcurrentWriteException(string message) : base(message)
        {
        }

        public EventStreamConcurrentWriteException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EventStreamConcurrentWriteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}