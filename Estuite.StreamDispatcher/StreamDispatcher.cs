using System.Threading;
using System.Threading.Tasks;

namespace Estuite.StreamDispatcher
{
    public class StreamDispatcher<TEvent> : IDispatchStreams
    {
        private readonly IConfirmEventsDispatched _dispatched;
        private readonly IPullEventsForDispatching<TEvent> _dispatching;
        private readonly IDispatchEvents<TEvent> _events;

        public StreamDispatcher(
            IPullEventsForDispatching<TEvent> dispatching,
            IDispatchEvents<TEvent> events,
            IConfirmEventsDispatched dispatched)
        {
            _dispatching = dispatching;
            _dispatched = dispatched;
            _events = events;
        }

        public async Task Dispatch(StreamId id, CancellationToken token = new CancellationToken())
        {
            while (true)
            {
                var events = await _dispatching.Pull(id, token);
                await _events.Dispatch(events, token);
                await _dispatched.Commit(token);
                if (token.IsCancellationRequested) return;
            }
        }
    }
}