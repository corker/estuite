using System;
using System.Runtime.Serialization;

namespace Estuite.StreamStore
{
    [Serializable]
    public class StreamNotFoundException : Exception
    {
        public StreamNotFoundException()
        {
        }

        public StreamNotFoundException(string message) : base(message)
        {
        }

        public StreamNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected StreamNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}