using System;

namespace Estuite.Domain
{
    public class Event
    {
        public Event(long version, object body)
        {
            if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Version = version;
        }

        public long Version { get; }

        public object Body { get; }
    }
}