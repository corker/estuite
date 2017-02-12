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
        IRegisterStreams,
        IHydrateAggregates,
        IHydrateStreams,
        ICommitAggregates
    {
        private readonly Dictionary<StreamId, IFlushEvents> _aggregates;
        private readonly BucketId _bucketId;
        private readonly ICreateSessions _createSessions;
        private readonly IGenerateIdentities _identities;
        private readonly IReadStreams _readStreams;
        private readonly ICreateStreamIdentities _streamIdentities;
        private readonly IWriteStreams _writeStreams;

        public UnitOfWork(
            BucketId bucketId,
            ICreateSessions createSessions,
            IWriteStreams writeStreams,
            IReadStreams readStreams)
        {
            if (bucketId == null) throw new ArgumentNullException(nameof(bucketId));
            _bucketId = bucketId;
            _createSessions = createSessions;
            _writeStreams = writeStreams;
            _readStreams = readStreams;
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

        public async Task<bool> TryHydrate(ICanBeHydrated aggregate, CancellationToken token = new CancellationToken())
        {
            return await aggregate.TryHydrateTo(this, token);
        }

        public async Task Hydrate<TId, TStream>(
            TId id,
            TStream stream,
            CancellationToken token = new CancellationToken())
            where TStream : IHydrateEvents, IFlushEvents
        {
            var streamId = _streamIdentities.Create<TId, TStream>(_bucketId, id);
            await _readStreams.Read(streamId, stream, token);
            _aggregates.Add(streamId, stream);
        }

        public async Task<bool> TryHydrate<TId, TStream>(
            TId id,
            TStream stream,
            CancellationToken token = new CancellationToken())
            where TStream : IHydrateEvents, IFlushEvents
        {
            var streamId = _streamIdentities.Create<TId, TStream>(_bucketId, id);
            var result = await _readStreams.TryRead(streamId, stream, token);
            _aggregates.Add(streamId, stream);
            return result;
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
            await _writeStreams.Write(session, token);
        }
    }
}