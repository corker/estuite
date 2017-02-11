namespace Estuite.Domain
{
    public interface ICanBeHydrated
    {
        void HydrateTo(IHydrateEventStreams streams);
    }
}