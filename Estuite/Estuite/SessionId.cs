using System;

namespace Estuite
{
    public class SessionId
    {
        public SessionId(Guid value)
        {
            if (value == Guid.Empty) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        public Guid Value { get; private set; }
    }
}