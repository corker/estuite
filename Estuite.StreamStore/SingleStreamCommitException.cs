using System;
using System.Runtime.Serialization;

namespace Estuite.StreamStore
{
    [Serializable]
    public class SingleStreamCommitException : Exception
    {
        public SingleStreamCommitException()
        {
        }

        public SingleStreamCommitException(string message) : base(message)
        {
        }

        public SingleStreamCommitException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SingleStreamCommitException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}