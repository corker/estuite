namespace Estuite.Domain
{
    public interface ICanBeRegistered
    {
        void RegisterTo(IRegisterEventStreams streams);
    }
}