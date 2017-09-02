using System;

namespace Estuite.StreamStore
{
    public class EventRecord
    {
        public EventRecord(long version, object body)
        {
            if (version <= 0) throw new ArgumentOutOfRangeException(nameof(version));
            Version = version;
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        public long Version { get; }
        public object Body { get; }
    }
}