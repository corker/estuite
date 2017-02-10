using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;

namespace Estuite
{
    public class UnitOfWork : IRegisterAggregates, IHydrateAggregates, ICommitAggregates
    {
        private readonly Dictionary<StreamId, IFlushEvents> _aggregates;
        private readonly BucketId _bucketId;
        private readonly ICreateSessions _createSessions;
        private readonly IGenerateIdentities _identities;
        private readonly ICreateStreamIdentities _streamIdentities;
        private readonly IWriteSessions _writeSessions;

        public UnitOfWork(BucketId bucketId, ICreateSessions createSessions, IWriteSessions writeSessions)
        {
            if (bucketId == null) throw new ArgumentNullException(nameof(bucketId));
            _bucketId = bucketId;
            _createSessions = createSessions;
            _writeSessions = writeSessions;
            _aggregates = new Dictionary<StreamId, IFlushEvents>(StreamIdEqualityComparer.Instance);
            _identities = this as IGenerateIdentities ?? new GuidCombGenerator(new UtcDateTimeProvider());
            _streamIdentities = this as ICreateStreamIdentities ?? new DefaultStreamIdentityFactory();
        }

        public async Task Commit(CancellationToken token = new CancellationToken())
        {
            var streamsToWrite = _aggregates
                .Select(x => new {StreamId = x.Key, Events = x.Value.Flush()})
                .Where(x => x.Events.Any())
                .ToArray();

            switch (streamsToWrite.Length)
            {
                case 0:
                    return;
                case 1:
                    var streamId = streamsToWrite[0].StreamId;
                    var events = streamsToWrite[0].Events;
                    await WriteStream(streamId, events, token);
                    break;
                default:
                    var ids = string.Join(", ", streamsToWrite.Select(x => x.StreamId));
                    string message = $"Can't commit multiple streams. Stream ids {ids}";
                    throw new InvalidOperationException(message);
            }
        }

        public void Hydrate<TId, TAggregate>(TId id, TAggregate events) where TAggregate : Aggregate<TId>
        {
            ((IHydrateEvents)events).Hydrate(new object[0]);
            throw new NotImplementedException();
        }

        public void Hydrate(ICanBeHydrated aggregate)
        {
            aggregate.HydrateWith(this);
        }

        public void Register(ICanBeRegistered aggregate)
        {
            aggregate.RegisterWith(this);
        }

        public void Register<TId, TAggregate>(TId id, TAggregate aggregate) where TAggregate : Aggregate<TId>
        {
            var streamId = _streamIdentities.Create<TId, TAggregate>(_bucketId, id);
            _aggregates.Add(streamId, aggregate);
        }

        private async Task WriteStream(StreamId streamId, IEnumerable<Event> events, CancellationToken token)
        {
            var sessionId = new SessionId($"{_identities.Generate()}");
            var session = _createSessions.Create(streamId, sessionId, events);
            await _writeSessions.Write(session, token);
        }
    }
}