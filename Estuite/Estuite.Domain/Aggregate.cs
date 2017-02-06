using System;
using System.Collections.Generic;

namespace Estuite.Domain
{
    public abstract class Aggregate<TId> : IFlushEvents, IHydrateEvents, ICanBeHydrated, ICanBeRegistered
    {
        private readonly ICreateEvents _eventFactory;
        private readonly IHandleEvents _eventHandler;
        private List<object> _events = new List<object>();

        private TId _id;

        protected Aggregate()
        {
            _eventFactory = this as ICreateEvents ?? new DefaultEventFactory();
            _eventHandler = this as IHandleEvents ?? new DefaultEventHandler();
        }

        void ICanBeHydrated.HydrateWith(IHydrateAggregates aggregates, object id)
        {
            aggregates.HydrateTo((TId) id, this);
        }

        void ICanBeRegistered.RegisterWith(IRegisterAggregates aggregates)
        {
            aggregates.Register(_id, this);
        }

        IEnumerable<object> IFlushEvents.Flush()
        {
            // http://stackoverflow.com/questions/1895761/test-for-equality-to-the-default-value
            if (EqualityComparer<TId>.Default.Equals(_id, default(TId)))
            {
                var message = "Can't flush events. Id for the aggregate was not set.";
                throw new InvalidOperationException(message);
            }
            var events = _events;
            _events = new List<object>();
            return events;
        }

        void IHydrateEvents.Hydrate(IEnumerable<object> events)
        {
            foreach (var @event in events) _eventHandler.Handle(this, @event);
        }

        protected void IdentifyAs(TId id)
        {
            // http://stackoverflow.com/questions/1895761/test-for-equality-to-the-default-value
            if (EqualityComparer<TId>.Default.Equals(id, default(TId)))
            {
                var message = "Can't identify the aggregate.";
                throw new ArgumentOutOfRangeException(nameof(id), message);
            }
            _id = id;
        }

        protected void Apply<TEvent>(Action<TEvent> action)
        {
            var @event = _eventFactory.Create<TEvent>();
            _eventHandler.Handle(this, @event);
            _events.Add(@event);
        }
    }
}