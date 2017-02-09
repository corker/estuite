using System;

namespace Estuite.Domain
{
    public class Event
    {
        public Event(int version, object body)
        {
            if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version));
            if (body == null) throw new ArgumentNullException(nameof(body));
            Body = body;
            Version = version;
        }

        public int Version { get; }

        public object Body { get; }
    }
}