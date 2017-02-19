namespace Estuite.StreamDispatcher.Azure
{
    public class AzureStreamDispatcher : StreamDispatcher<EventRecord>
    {
        public AzureStreamDispatcher(
            IPullEventsForDispatching<EventRecord> dispatching,
            IDispatchEvents<EventRecord> events,
            IConfirmEventsDispatched dispatched
        ) : base(dispatching, events, dispatched)
        {
        }
    }
}