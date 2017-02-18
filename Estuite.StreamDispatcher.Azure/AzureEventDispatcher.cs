using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public class AzureEventDispatcher : IDispatchEvents<EventRecord>
    {
        public async Task Dispatch(IEnumerable<EventRecord> events, CancellationToken token = new CancellationToken())
        {
            // ToDo: send to queue
            throw new NotImplementedException();
        }
    }
}