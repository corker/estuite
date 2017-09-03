using System;
using System.Runtime.Serialization;

namespace Estuite.StreamStore
{
    [Serializable]
    public class MultipleStreamsToCommitException : Exception
    {
        public MultipleStreamsToCommitException()
        {
        }

        public MultipleStreamsToCommitException(string message) : base(message)
        {
        }

        public MultipleStreamsToCommitException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MultipleStreamsToCommitException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}