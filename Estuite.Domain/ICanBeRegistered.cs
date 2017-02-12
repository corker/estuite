namespace Estuite.Domain
{
    public interface ICanBeRegistered
    {
        void RegisterTo(IRegisterStreams streams);
    }
}