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
            _id = Guid.NewGuid();
            _eventsToHydrate = _eventsToHydrate = new List<object>
            {
                new HydratedEvent {Index = 1},
                new HydratedEvent {Index = 2}
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
            context["then hydrate"] = () =>
            {
                act = () => _target.Hydrate(_eventsToHydrate);
                it["handles hydrated events"] = () => { _target.HydratedEvents.Count.ShouldBe(2); };
                it["handles hydrated events in correct order"] = () =>
                {
                    _target.HydratedEvents[0].Index.ShouldBe(1);
                    _target.HydratedEvents[1].Index.ShouldBe(2);
                };
                context["and events to hydrate are null"] = () =>
                {
                    before = () => _eventsToHydrate = null;
                    it["throws exception"] = expect<ArgumentNullException>(
                        "Value cannot be null.\r\nParameter name: events"
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
                    context["then flush"] = () =>
                    {
                        act = () => _flushedEvents = _target.Flush();
                        it["flushes applied events"] = () =>
                        {
                            _flushedEvents.Count.ShouldBe(_target.AppliedEvents.Count);
                            foreach (var @event in _flushedEvents) @event.Body.ShouldBeOfType<AppliedEvent>();
                        };
                        it["flushes applied events with versions after hydrated events"] = () =>
                        {
                            _flushedEvents[0].Version.ShouldBe(3);
                            _flushedEvents[1].Version.ShouldBe(4);
                        };
                        it["flushes applied events in correct order"] = () =>
                        {
                            ((AppliedEvent) _flushedEvents[0].Body).Index.ShouldBe(1);
                            ((AppliedEvent) _flushedEvents[1].Body).Index.ShouldBe(2);
                        };
                        context["then flush again"] = () =>
                        {
                            act = () => _flushedEvents = _target.Flush();
                            it["flushes no events"] = () => _flushedEvents.Count.ShouldBe(0);
                        };
                    };
                };
            };
        }

        private class AggregateUnderTest : Aggregate<Guid>
        {
            public readonly List<AppliedEvent> AppliedEvents = new List<AppliedEvent>();
            public readonly List<HydratedEvent> HydratedEvents = new List<HydratedEvent>();

            public AggregateUnderTest(Guid id) : base(id)
            {
            }

            public Guid ProvidedId => Id;
            public List<Event> Flush() => ((IFlushEvents) this).Flush();
            public void Hydrate(IEnumerable<object> events) => ((IHydrateEvents) this).Hydrate(events);

            public void ApplyTwoTimes()
            {
                Apply<AppliedEvent>(x => { x.Index = 1; });
                Apply<AppliedEvent>(x => { x.Index = 2; });
            }

            public void ApplyNull()
            {
                Apply<AppliedEvent>(null);
            }

            private void Handle(HydratedEvent @event)
            {
                HydratedEvents.Add(@event);
            }

            private void Handle(AppliedEvent @event)
            {
                AppliedEvents.Add(@event);
            }
        }

        private class HydratedEvent
        {
            public int Index { get; set; }
        }

        private class AppliedEvent
        {
            public int Index { get; set; }
        }

        private Guid _id;
        private AggregateUnderTest _target;
        private List<Event> _flushedEvents;
        private List<object> _eventsToHydrate;
    }
}