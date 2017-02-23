using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamStore.Azure
{
    public interface IReadEventToDispatchRecords
    {
        Task<IEnumerable<EventToDispatchRecordTableEntity>> Read(StreamId streamId, CancellationToken token);
    }
}