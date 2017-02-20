namespace Estuite.StreamDispatcher.Azure
{
    public class AzureStreamDispatcher : StreamDispatcher<EventRecordTableEntity>
    {
        public AzureStreamDispatcher(
            IPullEventsForDispatching<EventRecordTableEntity> dispatching,
            IDispatchEvents<EventRecordTableEntity> events,
            IConfirmEventsDispatched dispatched
        ) : base(dispatching, events, dispatched)
        {
        }
    }
}