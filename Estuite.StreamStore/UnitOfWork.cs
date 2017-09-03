using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;

namespace Estuite.StreamStore
{
    public class UnitOfWork : ICommitAggregates, IProvideAggregates
    {
        private static readonly MethodInfo GenericMethodGet;
        private readonly Dictionary<StreamId, IReceiveEvents> _aggregates;
        private readonly object _aggregatesLock;
        private readonly BucketId _bucketId;
        private readonly ICreateAggregates _createAggregates;
        private readonly ICreateStreamIdentities _identities;
        private readonly IReadStreams _readStreams;
        private readonly Dictionary<StreamId, ISendEvents> _writers;
        private readonly IWriteStreams _writeStreams;

        static UnitOfWork()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            GenericMethodGet = typeof(UnitOfWork).GetMethod("Get", flags);
        }

        public UnitOfWork(
            BucketId bucketId,
            IReadStreams readStreams,
            IWriteStreams writeStreams
        )
        {
            _bucketId = bucketId ?? throw new ArgumentNullException(nameof(bucketId));
            _writeStreams = writeStreams;
            _readStreams = readStreams;
            _aggregates = new Dictionary<StreamId, IReceiveEvents>(StreamIdEqualityComparer.Instance);
            _writers = new Dictionary<StreamId, ISendEvents>(StreamIdEqualityComparer.Instance);
            _aggregatesLock = new object();
            _createAggregates = this as ICreateAggregates ?? new DefaultAggregateFactory();
            _identities = this as ICreateStreamIdentities ?? new DefaultStreamIdentityFactory();
        }

        public async Task Commit(CancellationToken token)
        {
            EventReceiver[] receivers;
            lock (_aggregatesLock)
            {
                receivers = _writers.Select(x =>
                    {
                        var receiver = new EventReceiver(x.Key);
                        x.Value.SendTo(receiver);
                        return receiver;
                    })
                    .Where(x => x.HasEvents)
                    .ToArray();
            }
            switch (receivers.Length)
            {
                case 0:
                    return;
                case 1:
                    await receivers[0].WriteTo(_writeStreams);
                    return;
                default:
                    var ids = string.Join(@"
",receivers.Select(x => $"{x}"));
                    var message = $@"Can't commit changes from multiple streams.
{ids}";
                    throw new MultipleStreamsToCommitException(message);
            }
        }

        public async Task<T> Get<T>(object id, CancellationToken token) where T : IReceiveEvents
        {
            if (id == null) throw new ArgumentOutOfRangeException(nameof(id));
            var aggregateType = typeof(T);
            var idType = id.GetType();
            var genericMethodGet = GenericMethodGet.MakeGenericMethod(aggregateType, idType);
            return await (Task<T>) genericMethodGet.Invoke(this, new[] {id, token});
        }

        private async Task<T> Get<T, T1>(T1 id, CancellationToken token) where T : IReceiveEvents
        {
            if (id.IsNullOrEmpty()) throw new ArgumentOutOfRangeException(nameof(id));
            var streamId = _identities.Create<T>(_bucketId, id);
            lock (_aggregatesLock)
            {
                if (_aggregates.TryGetValue(streamId, out var value)) return (T) value;
            }
            var aggregate = _createAggregates.Create<T>(id);
            var receiver = new EventRecordReceiver(aggregate);
            await _readStreams.Read(streamId, receiver, token);
            lock (_aggregatesLock)
            {
                if (_aggregates.TryGetValue(streamId, out var value)) return (T) value;
                _aggregates.Add(streamId, aggregate);
                var writer = aggregate as ISendEvents;
                if (writer != null) _writers.Add(streamId, writer);
            }
            return aggregate;
        }

        private class EventReceiver : IReceiveEvents
        {
            private readonly List<Event> _events;
            private readonly StreamId _streamId;
            public override string ToString()
            {
                return $"StreamId {_streamId} with {_events.Count} event(s)";
            }

            public EventReceiver(StreamId streamId)
            {
                _streamId = streamId;
                _events = new List<Event>();
            }

            public bool HasEvents => _events.Any();

            public void Receive(IEnumerable<Event> events)
            {
                _events.AddRange(events);
            }

            public async Task WriteTo(IWriteStreams streams)
            {
                var records = _events.Select(x => new EventRecord(x.Version, x.Body)).ToArray();
                await streams.Write(_streamId, records);
            }
        }

        private class EventRecordReceiver : IReceiveEventRecords
        {
            private readonly IReceiveEvents _events;

            public EventRecordReceiver(IReceiveEvents events)
            {
                _events = events;
            }

            public void Receive(IEnumerable<EventRecord> records)
            {
                var events = records.Select(x => new Event(x.Version, x.Body));
                _events.Receive(events);
            }
        }
    }
}