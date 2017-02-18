using System;
using System.Runtime.Serialization;

namespace Estuite.StreamStore
{
    [Serializable]
    public class StreamConcurrentWriteException : Exception
    {
        public StreamConcurrentWriteException()
        {
        }

        public StreamConcurrentWriteException(string message) : base(message)
        {
        }

        public StreamConcurrentWriteException(string message, Exception inner) : base(message, inner)
        {
        }

        protected StreamConcurrentWriteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}