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
            _aggregates = new FakeIHydrateAggregates();
        }

        private void when_hydrate()
        {
            act = () => _target.HydrateWith(_aggregates);
            it["provides an id with expected type"] = () => { _aggregates.ProvidedId.ShouldBeOfType<string>(); };
            it["provides an expected id"] = () => { _aggregates.ProvidedId.ShouldBe(_id); };
            it["provides itself to hydrate aggregate"] = () => { _aggregates.ProvidedEvents.ShouldBeSameAs(_aggregate); };
            context["and hydrator is null"] = () =>
            {
                before = () => _aggregates = null;
                it["throw exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: aggregates"
                );
            };
        }

        private class FakeIHydrateAggregates : IHydrateAggregates
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
        private FakeIHydrateAggregates _aggregates;
    }
}