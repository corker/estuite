using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using NSpec;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_UnitOfWork")]
    public class describe_UnitOfWork_HydrateAggregate_WithNoEvents : nspec
    {
        private void before_each()
        {
            _bucketId = new BucketId("bucket-id");
            _aggregate = new FakeAggregate(Guid.NewGuid());
            _createSessions = new FakeICreateSessions();
            _readStreams = new FakeIReadStreams();
            _target = new UnitOfWork(_bucketId, _createSessions, null, _readStreams);
        }

        private void when_hydrate()
        {
            actAsync = async () => await _target.Hydrate(_aggregate);
            it["throws exception"] = expect<StreamNotFoundException>();
        }

        private void when_try_hydrate()
        {
            actAsync = async () => _returns = await _target.TryHydrate(_aggregate);
            it["returns false"] = () => _returns.ShouldBeFalse();
        }

        private class FakeAggregate : Aggregate<Guid>
        {
            public FakeAggregate(Guid id) : base(id)
            {
            }
        }

        private class FakeICreateSessions : ICreateSessions
        {
            public Session Create(StreamId streamId, SessionId sessionId, IEnumerable<Event> events)
            {
                var records = events.Select(x => new EventRecord {Payload = x.Body.GetType().Name}).ToArray();
                return new Session(streamId, sessionId, DateTime.Now, records);
            }
        }

        private class FakeIReadStreams : IReadStreams
        {
            public async Task Read(
                StreamId streamId,
                IHydrateEvents events,
                CancellationToken token = new CancellationToken())
            {
                throw new StreamNotFoundException();
            }

            public async Task<bool> TryRead(
                StreamId streamId,
                IHydrateEvents events,
                CancellationToken token = new CancellationToken())
            {
                return false;
            }
        }

        private UnitOfWork _target;
        private BucketId _bucketId;
        private FakeAggregate _aggregate;
        private FakeICreateSessions _createSessions;
        private FakeIReadStreams _readStreams;
        private bool _returns;
    }
}