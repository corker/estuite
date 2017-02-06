namespace Estuite.Domain
{
    public interface ICanBeHydrated
    {
        void HydrateWith(IHydrateAggregates aggregates, object id);
    }
}