namespace Estuite.StreamStore.Azure
{
    public interface ICreateEventRecordTableEntities
    {
        EventRecordTableEntity CreateFrom(EventRecord record);
    }
}