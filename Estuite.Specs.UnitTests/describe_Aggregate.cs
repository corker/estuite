using System;
using System.Collections.Generic;
using Estuite.Domain;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_Aggregate : nspec
    {
        private void before_each()
        {
            _id = Guid.Parse("efffb958-6be6-4337-add2-3b6658e2329e");
            _eventsToRead = new List<Event>
            {
                new Event(1, new ReceivedEvent {Index = 1}),
                new Event(2, new ReceivedEvent {Index = 2})
            };
        }

        private void when_create()
        {
            act = () => _target = new AggregateUnderTest(_id);
            it["is identified with id"] = () => _target.ProvidedId.ShouldBe(_id);
            context["and id is empty"] = () =>
            {
                before = () => _id = Guid.Empty;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: id"
                );
            };
            context["then read"] = () =>
            {
                act = () => _target.Read(_eventsToRead);
                it["handles received events"] = () => { _target.ReceivedEvents.Count.ShouldBe(2); };
                it["handles received events in correct order"] = () =>
                {
                    _target.ReceivedEvents[0].Index.ShouldBe(1);
                    _target.ReceivedEvents[1].Index.ShouldBe(2);
                };
                context["and events to read are null"] = () =>
                {
                    before = () => _eventsToRead = null;
                    it["throws exception"] = expect<ArgumentNullException>(
                        "Value cannot be null.\r\nParameter name: events"
                    );
                };
                context["and events are in a wrong order"] = () =>
                {
                    before = () => _eventsToRead = new List<Event> {new Event(2, new ReceivedEvent())};
                    it["throws exception"] = expect<InvalidEventVersionException>(
                        "Invalid event version received. AggregateUnderTest with id efffb958-6be6-4337-add2-3b6658e2329e, expected version 1, actual version 2"
                    );
                };
                context["then apply"] = () =>
                {
                    act = () => _target.ApplyTwoTimes();
                    it["handles applied events"] = () => { _target.AppliedEvents.Count.ShouldBe(2); };
                    context["and action to apply is null"] = () =>
                    {
                        act = () => _target.ApplyNull();
                        it["throws exception"] = expect<ArgumentNullException>(
                            "Value cannot be null.\r\nParameter name: action"
                        );
                    };
                    context["then write"] = () =>
                    {
                        act = () => _writtenEvents = _target.Flush();
                        it["write applied events"] = () =>
                        {
                            _writtenEvents.Count.ShouldBe(_target.AppliedEvents.Count);
                            foreach (var @event in _writtenEvents) @event.Body.ShouldBeOfType<AppliedEvent>();
                        };
                        it["write applied events with versions after received events"] = () =>
                        {
                            _writtenEvents[0].Version.ShouldBe(3);
                            _writtenEvents[1].Version.ShouldBe(4);
                        };
                        it["write applied events in correct order"] = () =>
                        {
                            ((AppliedEvent) _writtenEvents[0].Body).Index.ShouldBe(1);
                            ((AppliedEvent) _writtenEvents[1].Body).Index.ShouldBe(2);
                        };
                        context["then write again"] = () =>
                        {
                            act = () => _writtenEvents = _target.Flush();
                            it["write no events"] = () => _writtenEvents.Count.ShouldBe(0);
                        };
                    };
                };
            };
        }

        private class AggregateUnderTest : Aggregate<Guid>
        {
            public readonly List<AppliedEvent> AppliedEvents = new List<AppliedEvent>();
            public readonly List<ReceivedEvent> ReceivedEvents = new List<ReceivedEvent>();

            public AggregateUnderTest(Guid id) : base(id)
            {
            }

            public Guid ProvidedId => Id;

            public List<Event> Flush()
            {
                var events = new EventReceiver();
                ((ISendEvents) this).SendTo(events);
                return events;
            }

            public void Read(IEnumerable<Event> events)
            {
                ((IReceiveEvents) this).Receive(events);
            }

            public void ApplyTwoTimes()
            {
                Apply<AppliedEvent>(x => { x.Index = 1; });
                Apply<AppliedEvent>(x => { x.Index = 2; });
            }

            public void ApplyNull()
            {
                Apply<AppliedEvent>(null);
            }

            private void Handle(ReceivedEvent @event)
            {
                ReceivedEvents.Add(@event);
            }

            private void Handle(AppliedEvent @event)
            {
                AppliedEvents.Add(@event);
            }

            private class EventReceiver : List<Event>, IReceiveEvents
            {
                public void Receive(IEnumerable<Event> events)
                {
                    AddRange(events);
                }
            }
        }

        private class ReceivedEvent
        {
            public int Index { get; set; }
        }

        private class AppliedEvent
        {
            public int Index { get; set; }
        }

        private Guid _id;
        private AggregateUnderTest _target;
        private List<Event> _writtenEvents;
        private List<Event> _eventsToRead;
    }
}