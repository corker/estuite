using System;
using System.Collections.Generic;

namespace Estuite.Domain
{
    public abstract class Aggregate<TId> : IFlushEvents, IHydrateEvents, ICanBeHydrated, ICanBeRegistered
    {
        private readonly ICreateEvents _eventFactory;
        private readonly IHandleEvents _eventHandler;
        private List<Event> _events = new List<Event>();
        private int _version;

        protected Aggregate(TId id)
        {
            if (id.IsNullOrEmpty())
            {
                var message = "Can't create an aggregate with id as null or default value.";
                throw new ArgumentOutOfRangeException(nameof(id), message);
            }
            Id = id;
            _eventFactory = this as ICreateEvents ?? new DefaultEventFactory();
            _eventHandler = this as IHandleEvents ?? new DefaultEventHandler();
        }

        protected TId Id { get; }

        void ICanBeHydrated.HydrateWith(IHydrateAggregates aggregates)
        {
            aggregates.Hydrate(Id, this);
        }

        void ICanBeRegistered.RegisterWith(IRegisterAggregates aggregates)
        {
            aggregates.Register(Id, this);
        }

        List<Event> IFlushEvents.Flush()
        {
            var events = _events;
            _events = new List<Event>();
            return events;
        }

        void IHydrateEvents.Hydrate(IEnumerable<object> events)
        {
            foreach (var @event in events)
            {
                _eventHandler.Handle(this, @event);
                _version++;
            }
        }

        protected void Apply<TEvent>(Action<TEvent> action)
        {
            var @event = _eventFactory.Create<TEvent>();
            _eventHandler.Handle(this, @event);
            _version++;
            _events.Add(new Event(_version, @event));
        }
    }
}