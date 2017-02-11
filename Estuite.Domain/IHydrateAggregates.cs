namespace Estuite.Domain
{
    public interface IHydrateAggregates
    {
        void Hydrate(ICanBeHydrated aggregate);

        void Hydrate<TId, TAggregate>(TId id, TAggregate aggregate) where TAggregate : IHydrateEvents;
    }
}