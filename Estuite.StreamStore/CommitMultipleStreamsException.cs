using System;
using System.Runtime.Serialization;

namespace Estuite.StreamStore
{
    [Serializable]
    public class CommitMultipleStreamsException : Exception
    {
        public CommitMultipleStreamsException()
        {
        }

        public CommitMultipleStreamsException(string message) : base(message)
        {
        }

        public CommitMultipleStreamsException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CommitMultipleStreamsException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}