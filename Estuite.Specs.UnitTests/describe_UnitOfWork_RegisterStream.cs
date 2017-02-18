using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using Estuite.StreamStore;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_UnitOfWork")]
    public class describe_UnitOfWork_RegisterStream : nspec
    {
        private void before_each()
        {
            _id = Guid.Parse("bcb724f5-562f-4706-b470-570fa7174ec0");
            _bucketId = new BucketId("bucket-id");
            _createSessions = new FakeICreateSessions();
            _streams = new FakeIWriteStreams();
            _target = new UnitOfWork(_bucketId, null, _createSessions, _streams);
        }

        private void when_register()
        {
            context["aggregate with events"] = () =>
            {
                before = () => _aggregate = new FakeIFlushEvents(1);
                act = () => _target.Register(_id, _aggregate);
                context["and commit"] = () =>
                {
                    actAsync = async () => await _target.Commit();
                    it["has session"] = () => _streams.Sessions.Count.ShouldBe(1);
                    context["and session"] = () =>
                    {
                        act = () => _session = _streams.Sessions.Single();
                        it["has stream id"] = () => _session.StreamId.Value.ShouldBe(ExpectedStreamIdValue);
                        it["has created"] = () => _session.Created.ShouldBe(ExpectedCreated);
                        it["has event records"] = () => _session.Records.Length.ShouldBe(1);
                        context["and records"] = () =>
                        {
                            act = () => _records = _session.Records;
                            it["has version"] = () => _records[0].Version.ShouldBe(ExpectedEventVersion);
                            it["has payload"] = () => _records[0].Payload.ShouldBe("event-0");
                        };
                    };
                };
                context["and register another aggregate with events"] = () =>
                {
                    before = () => _anotherAggregate = new FakeIFlushEvents(1);
                    act = () => _target.Register(Guid.NewGuid(), _anotherAggregate);
                    context["and commit"] = () =>
                    {
                        actAsync = async () => await _target.Commit();
                        it["throws exception"] = expect<InvalidOperationException>();
                    };
                };
            };
            context["aggregate without events"] = () =>
            {
                before = () => _aggregate = new FakeIFlushEvents(0);
                act = () => _target.Register(_id, _aggregate);
                context["and commit"] = () =>
                {
                    actAsync = async () => await _target.Commit();
                    it["has no session"] = () => _streams.Sessions.Count.ShouldBe(0);
                };
            };
        }

        private void when_register_aggregate_with_no_events()
        {
            act = () => _target.Register(_id, _aggregate);
            context["and commit"] = () =>
            {
                actAsync = async () => await _target.Commit();
                it["has no sessions"] = () => _streams.Sessions.Count.ShouldBe(0);
            };
        }

        private class FakeIWriteStreams : IWriteStreams
        {
            public readonly List<Session> Sessions = new List<Session>();

            public Task Write(Session session, CancellationToken token = new CancellationToken())
            {
                Sessions.Add(session);
                return Task.FromResult(0);
            }
        }

        private class FakeICreateSessions : ICreateSessions
        {
            public Session Create(StreamId streamId, SessionId sessionId, IEnumerable<Event> events)
            {
                var records = events.Select(x => new EventRecord {Version = x.Version, Payload = $"{x.Body}"}).ToArray();
                return new Session(streamId, sessionId, ExpectedCreated, records);
            }
        }

        private class FakeIFlushEvents : IFlushEvents
        {
            private readonly int _count;

            public FakeIFlushEvents(int count)
            {
                _count = count;
            }

            public List<Event> Flush()
            {
                return Create(_count).ToList();
            }

            private static IEnumerable<Event> Create(int count)
            {
                for (var i = 0; i < count; i++) yield return new Event(i + 1, $"event-{i}");
            }
        }

        private UnitOfWork _target;
        private BucketId _bucketId;
        private ICreateSessions _createSessions;
        private FakeIWriteStreams _streams;
        private Guid _id;
        private FakeIFlushEvents _aggregate;
        private Session _session;
        private EventRecord[] _records;
        private static readonly DateTime ExpectedCreated = DateTime.Now;
        private FakeIFlushEvents _anotherAggregate;
        private const string ExpectedStreamIdValue = "bucket-id^FakeIFlushEvents^bcb724f5-562f-4706-b470-570fa7174ec0";
        private const int ExpectedEventVersion = 1;
    }
}