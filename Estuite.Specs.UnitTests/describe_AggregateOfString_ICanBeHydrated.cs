using System;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_Aggregate")]
    public class describe_AggregateOfString_ICanBeHydrated : nspec
    {
        private void before_each()
        {
            _id = "some id";
            _target = _aggregate = new AggregateUnderTest(_id);
            _eventStreams = new FakeIHydrateEventStreams();
        }

        private void when_hydrate()
        {
            act = () => _target.HydrateTo(_eventStreams);
            it["provides an id with expected type"] = () => { _eventStreams.ProvidedId.ShouldBeOfType<string>(); };
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

            public void Hydrate(ICanBeHydrated aggregate)
            {
                throw new NotImplementedException();
            }

            public void Hydrate<TId, TAggregate>(TId id, TAggregate aggregate) where TAggregate : IHydrateEvents
            {
                ProvidedId = id;
                ProvidedEvents = aggregate;
            }
        }

        private class AggregateUnderTest : Aggregate<string>
        {
            public AggregateUnderTest(string id) : base(id)
            {
            }
        }

        private string _id;
        private ICanBeHydrated _target;
        private AggregateUnderTest _aggregate;
        private FakeIHydrateEventStreams _eventStreams;
    }
}