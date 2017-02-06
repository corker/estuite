using System;

namespace Estuite.Domain
{
    public class Repository : IProvideAggregates, IRegisterAggregates, IHydrateAggregates
    {
        void IHydrateAggregates.HydrateTo<TId>(TId id, IHydrateEvents events)
        {
            events.Hydrate(new object[0]);
            throw new NotImplementedException();
        }

        public T Get<T>(object id) where T : ICanBeHydrated
        {
            var aggregate = Activator.CreateInstance<T>();
            aggregate.HydrateWith(this, id);
            return aggregate;
        }

        public void Register<T>(T aggregate) where T : ICanBeRegistered
        {
            aggregate.RegisterWith(this);
        }

        public void Register<T, TId>(TId id, T aggregate) where T : Aggregate<TId>
        {
            throw new NotImplementedException();
        }
    }
}