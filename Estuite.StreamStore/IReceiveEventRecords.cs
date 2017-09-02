using System.Collections.Generic;

namespace Estuite.StreamStore
{
    public interface IReceiveEventRecords
    {
        void Receive(IEnumerable<EventRecord> records);
    }
}