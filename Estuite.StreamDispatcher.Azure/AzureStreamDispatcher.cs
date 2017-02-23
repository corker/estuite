using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher.Azure
{
    public class AzureStreamDispatcher : IDispatchStreams
    {
        private readonly IConfirmDispatchedEvents _dispatched;
        private readonly IPullEventsForDispatching _dispatching;
        private readonly IDispatchEvents _events;

        public AzureStreamDispatcher(
            IPullEventsForDispatching dispatching, 
            IDispatchEvents events,
            IConfirmDispatchedEvents dispatched)
        {
            _dispatching = dispatching;
            _dispatched = dispatched;
            _events = events;
        }

        public async Task Dispatch(DispatchStreamJob job, CancellationToken token = new CancellationToken())
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            var events = await _dispatching.Pull(job.StreamId, token);
            while (events.Any())
            {
                await _events.Dispatch(events, token);
                await _dispatched.Confirm(token);
                if (token.IsCancellationRequested) return;
                events = await _dispatching.Pull(job.StreamId, token);
            }
        }
    }
}