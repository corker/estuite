namespace Estuite.StreamStore.Azure
{
    public interface IRestoreEventRecords
    {
        EventRecord RestoreFrom(EventRecordTableEntity entity);
    }
}