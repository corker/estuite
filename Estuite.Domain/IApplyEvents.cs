namespace Estuite.Domain
{
    public interface IApplyEvents
    {
        void Apply(object aggregate, object @event);
    }

    public interface IApplyEvents<in TAggregate>
    {
        void Apply<TEvent>(TAggregate aggregate, TEvent @event);
    }
}