using System;
using System.Runtime.Serialization;

namespace Estuite
{
    [Serializable]
    public class EventStreamNotFoundException : Exception
    {
        public EventStreamNotFoundException()
        {
        }

        public EventStreamNotFoundException(string message) : base(message)
        {
        }

        public EventStreamNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected EventStreamNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}