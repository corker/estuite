using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamStore;
using Estuite.StreamStore.Azure;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IPullEventsForDispatching
    {
        Task<List<EventToDispatchRecordTableEntity>> Pull(StreamId streamId, CancellationToken token = new CancellationToken());
    }
}