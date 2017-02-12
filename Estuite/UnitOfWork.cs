using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;

namespace Estuite
{
    public class UnitOfWork :
        IRegisterAggregates,
        IRegisterEventStreams,
        IHydrateAggregates,
        IHydrateEventStreams,
        ICommitAggregates
    {
        private readonly Dictionary<StreamId, IFlushEvents> _aggregates;
        private readonly BucketId _bucketId;
        private readonly ICreateSessions _createSessions;
        private readonly IGenerateIdentities _identities;
        private readonly IReadEventStreams _readEventStreams;
        private readonly ICreateStreamIdentities _streamIdentities;
        private readonly IWriteEventStreams _writeEventStreams;

        public UnitOfWork(
            BucketId bucketId,
            ICreateSessions createSessions,
            IWriteEventStreams writeEventStreams,
            IReadEventStreams readEventStreams)
        {
            if (bucketId == null) throw new ArgumentNullException(nameof(bucketId));
            _bucketId = bucketId;
            _createSessions = createSessions;
            _writeEventStreams = writeEventStreams;
            _readEventStreams = readEventStreams;
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
                    var ids = string.Join(", ", streamsToWrite.Select(x => x.StreamId.Value));
                    string message = $"Can't commit multiple event streams. Stream ids {ids}";
                    throw new InvalidOperationException(message);
            }
        }

        public async Task Hydrate(ICanBeHydrated aggregate, CancellationToken token = new CancellationToken())
        {
            await aggregate.HydrateTo(this, token);
        }

        public Task<bool> TryHydrate(ICanBeHydrated aggregate, CancellationToken token = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public async Task Hydrate<TId, TEventStream>(
            TId id,
            TEventStream stream,
            CancellationToken token = new CancellationToken())
            where TEventStream : IHydrateEvents, IFlushEvents
        {
            var streamId = _streamIdentities.Create<TId, TEventStream>(_bucketId, id);
            await _readEventStreams.Read(streamId, stream, token);
            _aggregates.Add(streamId, stream);
        }

        public async Task<bool> TryHydrate<TId, TEventStream>(
            TId id,
            TEventStream stream,
            CancellationToken token = new CancellationToken())
            where TEventStream : IHydrateEvents, IFlushEvents
        {
            throw new NotImplementedException();
        }

        public void Register(ICanBeRegistered aggregate)
        {
            aggregate.RegisterTo(this);
        }

        public void Register<TId, TEventStream>(TId id, TEventStream stream) where TEventStream : IFlushEvents
        {
            var streamId = _streamIdentities.Create<TId, TEventStream>(_bucketId, id);
            _aggregates.Add(streamId, stream);
        }

        private async Task WriteStream(
            StreamId streamId,
            IEnumerable<Event> events,
            CancellationToken token = new CancellationToken())
        {
            var sessionId = new SessionId($"{_identities.Generate()}");
            var session = _createSessions.Create(streamId, sessionId, events);
            await _writeEventStreams.Write(session, token);
        }
    }
}