namespace Estuite.Domain
{
    public interface IHydrateAggregates
    {
        void Hydrate(ICanBeHydrated aggregate);

    }
}