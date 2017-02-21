using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamStore;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IReadEventRecords
    {
        Task<IEnumerable<DispatchEventRecordTableEntity>> Read(StreamId streamId, CancellationToken token);
    }
}