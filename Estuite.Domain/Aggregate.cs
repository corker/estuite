using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Estuite.Domain
{
    public abstract class Aggregate<TId> : IFlushEvents, IHydrateEvents, ICanReadStreams, ICanBeRegistered
    {
        private readonly IApplyEvents _eventApplier;
        private readonly ICreateEvents _eventFactory;
        private List<Event> _events = new List<Event>();
        private int _version;

        protected Aggregate(TId id) : this(id, new DefaultEventFactory(), new DefaultEventApplier())
        {
        }

        protected Aggregate(TId id, ICreateEvents eventFactory, IApplyEvents eventApplier)
        {
            if (id.IsNullOrEmpty()) throw new ArgumentOutOfRangeException(nameof(id));
            Id = id;
            _eventFactory = eventFactory ?? throw new ArgumentNullException(nameof(eventFactory));
            _eventApplier = eventApplier ?? throw new ArgumentNullException(nameof(eventApplier));
        }

        protected TId Id { get; }

        async Task ICanReadStreams.ReadFrom(IReadStreams streams, CancellationToken token)
        {
            if (streams == null) throw new ArgumentNullException(nameof(streams));
            await streams.ReadInto(Id, this, token);
        }

        async Task<bool> ICanReadStreams.TryReadFrom(IReadStreams streams, CancellationToken token)
        {
            if (streams == null) throw new ArgumentNullException(nameof(streams));
            return await streams.TryReadInto(Id, this, token);
        }

        void ICanBeRegistered.RegisterWith(IRegisterStreams streams)
        {
            if (streams == null) throw new ArgumentNullException(nameof(streams));
            streams.Register(Id, this);
        }

        List<Event> IFlushEvents.Flush()
        {
            var events = _events;
            _events = new List<Event>();
            return events;
        }

        void IHydrateEvents.Hydrate(IEnumerable<object> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            foreach (var @event in events)
            {
                _eventApplier.Apply(this, @event);
                _version++;
            }
        }

        protected void Apply<TEvent>(Action<TEvent> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            var @event = _eventFactory.Create<TEvent>();
            action(@event);
            _eventApplier.Apply(this, @event);
            _version++;
            _events.Add(new Event(_version, @event));
        }
    }
}