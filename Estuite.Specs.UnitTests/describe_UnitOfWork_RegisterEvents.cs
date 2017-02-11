using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_UnitOfWork")]
    public class describe_UnitOfWork_RegisterEvents : nspec
    {
        private void before_each()
        {
            _id = Guid.Parse("bcb724f5-562f-4706-b470-570fa7174ec0");
            _bucketId = new BucketId("bucket-id");
            _createSessions = new FakeICreateSessions();
            _writeSessions = new FakeIWriteSessions();
            _events = new FakeIFlushEvents();
            _target = new UnitOfWork(_bucketId, _createSessions, _writeSessions);
        }

        private void when_register_aggregate_with_events()
        {
            act = () => _target.Register(_id, _events);
            context["and commit"] = () =>
            {
                actAsync = async () => await _target.Commit();
                it["has session"] = () => _writeSessions.Sessions.Count.ShouldBe(1);
                context["and session"] = () =>
                {
                    act = () => _session = _writeSessions.Sessions.Single();
                    it["has stream id"] = () => _session.StreamId.Value.ShouldBe(ExpectedStreamIdValue);
                    it["has created"] = () => _session.Created.ShouldBe(ExpectedCreated);
                    it["has event records"] = () => _session.Records.Length.ShouldBe(1);
                    context["and records"] = () =>
                    {
                        act = () => _records = _session.Records;
                        it["has version"] = () => _records[0].Version.ShouldBe(ExpectedEventVersion);
                        it["has payload"] = () => _records[0].Payload.ShouldBe(ExpectedEventBody);
                    };
                };
            };
            context["and register another aggregate with events"] = () =>
            {
                act = () => _target.Register(Guid.NewGuid(), _events);
                context["and commit"] = () =>
                {
                    actAsync = async () => await _target.Commit();
                    it["throws exception"] = expect<InvalidOperationException>();
                };
            };
        }

        private class FakeIWriteSessions : IWriteSessions
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
            public Session Create(StreamId streamId, SessionId sessionId, IEnumerable<Event> @event)
            {
                var records = @event.Select(x => new EventRecord {Version = x.Version, Payload = $"{x.Body}"}).ToArray();
                return new Session(streamId, sessionId, ExpectedCreated, records);
            }
        }

        private class FakeIFlushEvents : IFlushEvents
        {
            public List<Event> Flush()
            {
                return new List<Event> {new Event(ExpectedEventVersion, ExpectedEventBody)};
            }
        }

        private UnitOfWork _target;
        private BucketId _bucketId;
        private ICreateSessions _createSessions;
        private FakeIWriteSessions _writeSessions;
        private Guid _id;
        private FakeIFlushEvents _events;
        private Session _session;
        private EventRecord[] _records;
        private static readonly DateTime ExpectedCreated = DateTime.Now;
        private const string ExpectedStreamIdValue = "bucket-id^FakeIFlushEvents^bcb724f5-562f-4706-b470-570fa7174ec0";
        private const int ExpectedEventVersion = 1;
        private const string ExpectedEventBody = "event";
    }
}