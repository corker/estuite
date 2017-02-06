namespace Estuite.Domain
{
    public interface IHydrateAggregates
    {
        void HydrateTo<TId>(TId id, IHydrateEvents events);
    }
}