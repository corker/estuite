using System;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_Aggregate")]
    public class describe_AggregateOfInteger_ICanBeHydrated : nspec
    {
        private void before_each()
        {
            _id = 1;
            _target = _aggregate = new AggregateUnderTest(_id);
            _eventStreams = new FakeIHydrateEventStreams();
        }

        private void when_hydrate()
        {
            act = () => _target.HydrateTo(_eventStreams);
            it["provides an id with expected type"] = () => { _eventStreams.ProvidedId.ShouldBeOfType<int>(); };
            it["provides an expected id"] = () => { _eventStreams.ProvidedId.ShouldBe(_id); };
            it["provides itself to hydrate aggregate"] = () => { _eventStreams.ProvidedEvents.ShouldBeSameAs(_aggregate); };
            context["and hydrator is null"] = () =>
            {
                before = () => _eventStreams = null;
                it["throw exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: streams"
                );
            };
        }

        private class FakeIHydrateEventStreams : IHydrateEventStreams
        {
            public IHydrateEvents ProvidedEvents { get; private set; }
            public object ProvidedId { get; private set; }

            public void Hydrate<TId, TAggregate>(TId id, TAggregate aggregate) where TAggregate : IHydrateEvents, IFlushEvents
            {
                ProvidedId = id;
                ProvidedEvents = aggregate;
            }
        }

        private class AggregateUnderTest : Aggregate<int>
        {
            public AggregateUnderTest(int id) : base(id)
            {
            }
        }

        private int _id;
        private ICanBeHydrated _target;
        private AggregateUnderTest _aggregate;
        private FakeIHydrateEventStreams _eventStreams;
    }
}