using System;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_Aggregate")]
    public class describe_AggregateOfObject_ICanBeHydrated : nspec
    {
        private void before_each()
        {
            _id = new object();
            _target = _aggregate = new AggregateUnderTest(_id);
            _aggregates = new FakeIHydrateAggregates();
        }

        private void when_hydrate()
        {
            act = () => _target.HydrateWith(_aggregates);
            it["provides an id with expected type"] = () => { _aggregates.ProvidedId.ShouldBeOfType<object>(); };
            it["provides an expected id"] = () => { _aggregates.ProvidedId.ShouldBe(_id); };
            it["provides itself to hydrate events"] = () => { _aggregates.ProvidedEvents.ShouldBeSameAs(_aggregate); };
        }

        private class FakeIHydrateAggregates : IHydrateAggregates
        {
            public IHydrateEvents ProvidedEvents { get; private set; }
            public object ProvidedId { get; private set; }

            public void Hydrate(ICanBeHydrated aggregate)
            {
                throw new NotImplementedException();
            }

            public void Hydrate<TId>(TId id, IHydrateEvents events)
            {
                ProvidedId = id;
                ProvidedEvents = events;
            }
        }

        private class AggregateUnderTest : Aggregate<object>
        {
            public AggregateUnderTest(object id) : base(id)
            {
            }
        }

        private object _id;
        private ICanBeHydrated _target;
        private AggregateUnderTest _aggregate;
        private FakeIHydrateAggregates _aggregates;
    }
}