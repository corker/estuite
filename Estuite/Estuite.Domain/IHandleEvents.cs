namespace Estuite.Domain
{
    public interface IHandleEvents
    {
        void Handle(object aggregate, object @event);
    }
}