namespace Estuite.Domain
{
    public interface ICreateEvents
    {
        TEvent Create<TEvent>();
    }
}