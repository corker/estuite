using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;

namespace Estuite.StreamStore
{
    public class UnitOfWork :
        IRegisterAggregates,
        IRegisterStreams,
        IHydrateAggregates,
        Domain.IReadStreams,
        ICommitAggregates
    {
        private readonly Dictionary<StreamId, IFlushEvents> _aggregates;
        private readonly object _aggregatesLock;
        private readonly BucketId _bucketId;
        private readonly ICreateSessions _createSessions;
        private readonly IGenerateIdentities _identities;
        private readonly IReadStreams _readStreams;
        private readonly ICreateStreamIdentities _streamIdentities;
        private readonly IWriteStreams _writeStreams;

        public UnitOfWork(
            BucketId bucketId,
            IReadStreams readStreams,
            ICreateSessions createSessions,
            IWriteStreams writeStreams)
        {
            if (bucketId == null) throw new ArgumentNullException(nameof(bucketId));
            _bucketId = bucketId;
            _createSessions = createSessions;
            _writeStreams = writeStreams;
            _readStreams = readStreams;
            _aggregates = new Dictionary<StreamId, IFlushEvents>(StreamIdEqualityComparer.Instance);
            _aggregatesLock = new object();
            _identities = this as IGenerateIdentities ?? new GuidCombGenerator(new UtcDateTimeProvider());
            _streamIdentities = this as ICreateStreamIdentities ?? new DefaultStreamIdentityFactory();
        }

        public async Task Commit(CancellationToken token)
        {
            Tuple<StreamId, List<Event>>[] streamsToWrite;

            lock (_aggregatesLock)
            {
                streamsToWrite = _aggregates
                    .Select(x => new Tuple<StreamId, List<Event>>(x.Key, x.Value.Flush()))
                    .Where(x => x.Item2.Any())
                    .ToArray();
            }

            switch (streamsToWrite.Length)
            {
                case 0:
                    return;
                case 1:
                    var streamId = streamsToWrite[0].Item1;
                    var events = streamsToWrite[0].Item2;
                    await WriteStream(streamId, events, token);
                    break;
                default:
                    var ids = string.Join(", ", streamsToWrite.Select(x => x.Item1.Value));
                    string message = $"Can't commit multiple event streams. Stream ids {ids}";
                    throw new InvalidOperationException(message);
            }
        }

        public async Task Hydrate(ICanReadStreams aggregate, CancellationToken token)
        {
            await aggregate.ReadFrom(this, token);
        }

        public async Task<bool> TryHydrate(ICanReadStreams aggregate, CancellationToken token)
        {
            return await aggregate.TryReadFrom(this, token);
        }

        public async Task ReadInto<TId, TStream>(TId id, TStream stream, CancellationToken token)
            where TStream : IHydrateEvents, IFlushEvents
        {
            var type = stream.GetType();
            var streamId = _streamIdentities.Create(_bucketId, id, type);
            await _readStreams.Read(streamId, stream, token);
            lock (_aggregatesLock)
            {
                _aggregates.Add(streamId, stream);
            }
        }

        public async Task<bool> TryReadInto<TId, TStream>(TId id, TStream stream, CancellationToken token)
            where TStream : IHydrateEvents, IFlushEvents
        {
            var type = stream.GetType();
            var streamId = _streamIdentities.Create(_bucketId, id, type);
            var result = await _readStreams.TryRead(streamId, stream, token);
            lock (_aggregatesLock)
            {
                _aggregates.Add(streamId, stream);
            }
            return result;
        }

        public void Register(ICanBeRegistered aggregate)
        {
            aggregate.RegisterWith(this);
        }

        public void Register<TId, TEventStream>(TId id, TEventStream stream) where TEventStream : IFlushEvents
        {
            var type = stream.GetType();
            var streamId = _streamIdentities.Create(_bucketId, id, type);
            lock (_aggregatesLock)
            {
                _aggregates.Add(streamId, stream);
            }
        }

        private async Task WriteStream(StreamId streamId, IEnumerable<Event> events, CancellationToken token)
        {
            var sessionId = new SessionId($"{_identities.Generate()}");
            var session = _createSessions.Create(streamId, sessionId, events);
            await _writeStreams.Write(session, token);
        }
    }
}