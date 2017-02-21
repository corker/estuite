using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.StreamStore;

namespace Estuite.StreamDispatcher
{
    public abstract class StreamDispatcher<TEvent> : IDispatchStreams
    {
        private readonly IConfirmEventsDispatched _dispatched;
        private readonly IPullEventsForDispatching<TEvent> _dispatching;
        private readonly IDispatchEvents<TEvent> _events;

        protected StreamDispatcher(
            IPullEventsForDispatching<TEvent> dispatching,
            IDispatchEvents<TEvent> events,
            IConfirmEventsDispatched dispatched)
        {
            _dispatching = dispatching;
            _dispatched = dispatched;
            _events = events;
        }

        public async Task Dispatch(StreamDispatchJob job, CancellationToken token = new CancellationToken())
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