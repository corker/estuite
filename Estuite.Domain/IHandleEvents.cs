namespace Estuite.Domain
{
    public interface IHandleEvents
    {
        void Handle(object aggregate, object @event);
    }

    public interface IHandleEvents<in TAggregate>
    {
        void Handle<TEvent>(TAggregate aggregate, TEvent @event);
    }
}