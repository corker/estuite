using System;
using System.Runtime.Serialization;

namespace Estuite.Domain
{
    [Serializable]
    public class InvalidEventVersionException : Exception
    {
        public InvalidEventVersionException()
        {
        }

        public InvalidEventVersionException(string message) : base(message)
        {
        }

        public InvalidEventVersionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidEventVersionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}