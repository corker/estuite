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
            _aggregates = new FakeIHydrateAggregates();
        }

        private void when_hydrate()
        {
            act = () => _target.HydrateWith(_aggregates);
            it["provides an id with expected type"] = () => { _aggregates.ProvidedId.ShouldBeOfType<int>(); };
            it["provides an expected id"] = () => { _aggregates.ProvidedId.ShouldBe(_id); };
            it["provides itself to hydrate events"] = () => { _aggregates.ProvidedEvents.ShouldBeSameAs(_aggregate); };
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

            public void Hydrate<TId, TAggregate>(TId id, TAggregate events) where TAggregate : Aggregate<TId>
            {
                ProvidedId = id;
                ProvidedEvents = events;
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
        private FakeIHydrateAggregates _aggregates;
    }
}