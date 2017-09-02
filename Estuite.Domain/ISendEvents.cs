namespace Estuite.Domain
{
    public interface ISendEvents
    {
        void SendTo(IReceiveEvents events);
    }
}