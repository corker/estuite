namespace Estuite.Domain
{
    public interface IProvideAggregates
    {
        T Get<T>(object id) where T : ICanBeHydrated;
    }
}