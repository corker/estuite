using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamStore.Azure;

namespace Estuite.StreamDispatcher.Azure
{
    public interface IDispatchEvents
    {
        Task Dispatch(List<EventToDispatchRecordTableEntity> events, CancellationToken token = new CancellationToken());
    }
}