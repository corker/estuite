using System;
using System.Collections.Generic;

namespace Estuite.Domain
{
    public abstract class Aggregate<TId> : IFlushEvents, IHydrateEvents, ICanBeHydrated, ICanBeRegistered
    {
        private readonly ICreateEvents _eventFactory;
        private readonly IHandleEvents _eventHandler;
        private readonly TId _id;
        private List<object> _events = new List<object>();

        protected Aggregate(TId id)
        {
            if (_id.IsNullOrEmpty())
            {
                var message = "Can't create an aggregate with id as null or default value.";
                throw new ArgumentOutOfRangeException(nameof(id));
            }
            _id = id;
            _eventFactory = this as ICreateEvents ?? new DefaultEventFactory();
            _eventHandler = this as IHandleEvents ?? new DefaultEventHandler();
        }

        void ICanBeHydrated.HydrateWith(IHydrateAggregates aggregates)
        {
            aggregates.Hydrate(_id, this);
        }

        void ICanBeRegistered.RegisterWith(IRegisterAggregates aggregates)
        {
            aggregates.Register(_id, this);
        }

        IEnumerable<object> IFlushEvents.Flush()
        {
            var events = _events;
            _events = new List<object>();
            return events;
        }

        void IHydrateEvents.Hydrate(IEnumerable<object> events)
        {
            foreach (var @event in events) _eventHandler.Handle(this, @event);
        }

        protected void Apply<TEvent>(Action<TEvent> action)
        {
            var @event = _eventFactory.Create<TEvent>();
            _eventHandler.Handle(this, @event);
            _events.Add(@event);
        }
    }
}