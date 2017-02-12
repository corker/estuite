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
    public class describe_UnitOfWork_HydrateAggregate_WithEvents : nspec
    {
        private void before_each()
        {
            _id = Guid.Parse("bcb724f5-562f-4706-b470-570fa7174ec0");
            _bucketId = new BucketId("bucket-id");
            _aggregate = new FakeAggregate(_id);
            _createSessions = new FakeICreateSessions();
            _writeSessionStreams = new FakeIWriteStreams();
            _readStreams = new FakeIReadStreams();
            _target = new UnitOfWork(_bucketId, _createSessions, _writeSessionStreams, _readStreams);
        }

        private void when_hydrate()
        {
            actAsync = async () => await _target.Hydrate(_aggregate);
            it["hydrates event on aggregate"] = () => _aggregate.IsHydrated.ShouldBeTrue();
            context["when apply event"] = () =>
            {
                act = () => _aggregate.ApplyEvents();
                it["applies event on aggregate"] = () => _aggregate.IsApplied.ShouldBeTrue();
                context["when commit"] = () =>
                {
                    actAsync = async () => await _target.Commit();
                    it["writes only applied event"] = () => { _writeSessionStreams.HasOnlyAppliedEvent.ShouldBeTrue(); };
                };
            };
        }

        private class FakeAggregate : Aggregate<Guid>
        {
            public FakeAggregate(Guid id) : base(id)
            {
            }

            public bool IsHydrated { get; private set; }

            public bool IsApplied { get; private set; }

            public void ApplyEvents()
            {
                Apply<FakeAppliedEvent>(x => { });
            }

            private void Handle(FakeHydratedEvent @event)
            {
                IsHydrated = true;
            }

            private void Handle(FakeAppliedEvent @event)
            {
                IsApplied = true;
            }
        }

        private class FakeHydratedEvent
        {
        }

        private class FakeAppliedEvent
        {
        }

        private class FakeICreateSessions : ICreateSessions
        {
            public Session Create(StreamId streamId, SessionId sessionId, IEnumerable<Event> events)
            {
                var records = events.Select(x => new EventRecord {Payload = x.Body.GetType().Name}).ToArray();
                return new Session(streamId, sessionId, DateTime.Now, records);
            }
        }

        private class FakeIWriteStreams : IWriteStreams
        {
            private readonly List<EventRecord> _records = new List<EventRecord>();

            public bool HasOnlyAppliedEvent
            {
                get { return _records.All(x => x.Payload == typeof(FakeAppliedEvent).Name); }
            }

            public async Task Write(Session session, CancellationToken token = new CancellationToken())
            {
                _records.AddRange(session.Records);
            }
        }

        private class FakeIReadStreams : IReadStreams
        {
            public async Task Read(
                StreamId streamId,
                IHydrateEvents events,
                CancellationToken token = new CancellationToken())
            {
                events.Hydrate(new List<object> {new FakeHydratedEvent()});
            }

            public async Task<bool> TryRead(
                StreamId streamId,
                IHydrateEvents events,
                CancellationToken token = new CancellationToken())
            {
                throw new NotImplementedException();
            }
        }

        private UnitOfWork _target;
        private Guid _id;
        private BucketId _bucketId;
        private FakeAggregate _aggregate;
        private FakeICreateSessions _createSessions;
        private FakeIWriteStreams _writeSessionStreams;
        private FakeIReadStreams _readStreams;
    }
}