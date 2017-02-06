namespace Estuite
{
    public interface ICreateEvents
    {
        TEvent Create<TEvent>();
    }
}