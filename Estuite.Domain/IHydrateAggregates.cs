namespace Estuite.Domain
{
    public interface IHydrateAggregates
    {
        void Hydrate(ICanBeHydrated aggregate);

        void Hydrate<TId>(TId id, IHydrateEvents events);
    }
}