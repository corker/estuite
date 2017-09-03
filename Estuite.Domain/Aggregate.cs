using System;
using System.Collections.Generic;

namespace Estuite.Domain
{
    public abstract class Aggregate<TId> : IReceiveEvents, ISendEvents
    {
        private readonly IApplyEvents _eventApplier;
        private readonly ICreateEvents _eventFactory;
        private readonly object _eventsLock;
        private List<Event> _events = new List<Event>();
        private long _version;

        protected Aggregate(TId id) : this(id, new DefaultEventFactory(), new DefaultEventApplier())
        {
        }

        protected Aggregate(TId id, ICreateEvents eventFactory, IApplyEvents eventApplier)
        {
            if (id.IsNullOrEmpty()) throw new ArgumentOutOfRangeException(nameof(id));
            Id = id;
            _eventFactory = eventFactory ?? throw new ArgumentNullException(nameof(eventFactory));
            _eventApplier = eventApplier ?? throw new ArgumentNullException(nameof(eventApplier));
            _eventsLock = new object();
        }

        protected TId Id { get; }

        void IReceiveEvents.Receive(IEnumerable<Event> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            foreach (var @event in events)
            {
                _eventApplier.Apply(this, @event.Body);
                _version++;
                if (_version != @event.Version)
                    throw new InvalidEventVersionException(
                        $"Invalid event version received. {GetType().Name} with id {Id}, expected version {_version}, actual version {@event.Version}"
                    );
            }
        }

        void ISendEvents.SendTo(IReceiveEvents receiver)
        {
            lock (_eventsLock)
            {
                var events = _events;
                _events = new List<Event>();
                receiver.Receive(events);
            }
        }

        protected void Apply<TEvent>(Action<TEvent> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var @event = _eventFactory.Create<TEvent>();
            action(@event);
            lock (_eventsLock)
            {
                _eventApplier.Apply(this, @event);
                _version++;
                _events.Add(new Event(_version, @event));
            }
        }
    }
}