using System;
using System.Runtime.Serialization;

namespace Estuite.Domain
{
    [Serializable]
    public class UnexpectedEventVersionException : Exception
    {
        public UnexpectedEventVersionException()
        {
        }

        public UnexpectedEventVersionException(string message) : base(message)
        {
        }

        public UnexpectedEventVersionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnexpectedEventVersionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}