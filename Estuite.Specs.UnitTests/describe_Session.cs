using System;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_Session : nspec
    {
        private void before_each()
        {
            _records = new EventRecord[0];
            _sessionId = new SessionId("session-id");
            _created = DateTime.Parse("7 Apr 1953");
            _streamId = new StreamId(
                new BucketId("bucket-id"),
                new AggregateType("aggregate-type"),
                new AggregateId("aggregate-id")
            );
        }

        private void when_create()
        {
            act = () => _session = new Session(_streamId, _sessionId, _created, _records);
            it["has stream id"] = () => _session.StreamId.ShouldBe(_streamId);
            it["has session id"] = () => _session.SessionId.ShouldBe(_sessionId);
            it["has created"] = () => _session.Created.ShouldBe(_created);
            it["has records"] = () => _session.Created.ShouldBe(_created);
            context["and stream id is null"] = () =>
            {
                before = () => _streamId = null;
                it["throws exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: streamId"
                );
            };
            context["and session id is null"] = () =>
            {
                before = () => _sessionId = null;
                it["throws exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: sessionId"
                );
            };
            context["and created is default"] = () =>
            {
                before = () => _created = default(DateTime);
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: created"
                );
            };
            context["and records is null"] = () =>
            {
                before = () => _records = null;
                it["throws exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: records"
                );
            };
        }

        private Session _session;
        private EventRecord[] _records;
        private SessionId _sessionId;
        private DateTime _created;
        private StreamId _streamId;
    }
}