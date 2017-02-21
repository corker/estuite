namespace Estuite.StreamDispatcher.Azure
{
    public class AzureStreamDispatcher : StreamDispatcher<DispatchEventRecordTableEntity>
    {
        public AzureStreamDispatcher(
            IPullEventsForDispatching<DispatchEventRecordTableEntity> dispatching,
            IDispatchEvents<DispatchEventRecordTableEntity> events,
            IConfirmEventsDispatched dispatched
        ) : base(dispatching, events, dispatched)
        {
        }
    }
}