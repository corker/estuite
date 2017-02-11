using System;
using System.Collections.Generic;
using Estuite.Domain;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_SessionFactory : nspec
    {
        private void before_each()
        {
            _sessionId = new SessionId("session-id");
            _streamId = new StreamId(
                new BucketId("bucket-id"),
                new AggregateType("aggregate-type"),
                new AggregateId("aggregate-id")
            );
            _dateTime = new FakeIProvideUtcDateTime();
            _serializeEvents = new FakeISerializeEvents();
            _target = new SessionFactory(_dateTime, _serializeEvents);
        }

        private void when_create()
        {
            before = () => _events = new[] {new Event(1, new object())};
            act = () => _session = _target.Create(_streamId, _sessionId, _events);
            it["has expected session id"] = () => _session.SessionId.ShouldBe(_sessionId);
            it["has expected stream id"] = () => _session.StreamId.ShouldBe(_streamId);
            it["has expected created"] = () => _session.Created.ShouldBe(ExpectedCreated);
            it["has event records with session id"] = () => _session.Records[0].SessionId.ShouldBe(_sessionId);
            it["has event records with created"] = () => _session.Records[0].Created.ShouldBe(ExpectedCreated);
            it["has event records with type"] = () => _session.Records[0].Type.ShouldBe(ExpectedType);
            it["has event records with payload"] = () => _session.Records[0].Payload.ShouldBe(ExpectedPayload);
            context["and multiple events provided"] = () =>
            {
                before = () => _events = new[] {new Event(1, new object()), new Event(2, new object())};
                it["has all event records"] = () => _session.Records.Length.ShouldBe(2);
            };
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
            context["and events is null"] = () =>
            {
                before = () => _events = null;
                it["throws exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: events"
                );
            };
        }

        private class FakeIProvideUtcDateTime : IProvideUtcDateTime
        {
            public DateTime Now => ExpectedCreated;
        }

        private class FakeISerializeEvents : ISerializeEvents
        {
            public SerializedEvent Serialize(object @event)
            {
                return new SerializedEvent(ExpectedType, ExpectedPayload);
            }
        }

        private SessionFactory _target;
        private IProvideUtcDateTime _dateTime;
        private ISerializeEvents _serializeEvents;
        private IEnumerable<Event> _events;
        private Session _session;
        private SessionId _sessionId;
        private StreamId _streamId;
        private static readonly DateTime ExpectedCreated = DateTime.Parse("7 Apr 1953");
        private static readonly string ExpectedType = "event-type";
        private static readonly string ExpectedPayload = "event-payload";
    }
}