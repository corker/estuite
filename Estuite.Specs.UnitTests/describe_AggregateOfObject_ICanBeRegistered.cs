using System;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_Aggregate")]
    public class describe_AggregateOfObject_ICanBeRegistered : nspec
    {
        private void before_each()
        {
            _id = new object();
            _target = _aggregate = new AggregateUnderTest(_id);
            _registerer = new FakeIRegisterStreams();
        }

        private void when_register()
        {
            act = () => _target.RegisterWith(_registerer);
            it["provides an id with expected type"] = () => { _registerer.ProvidedId.ShouldBeOfType<object>(); };
            it["provides an expected id"] = () => { _registerer.ProvidedId.ShouldBe(_id); };
            it["provides itself to flush aggregate"] = () => { _registerer.ProvidedEvents.ShouldBeSameAs(_aggregate); };
            context["and registerer is null"] = () =>
            {
                before = () => _registerer = null;
                it["throw exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: streams"
                );
            };
        }

        private class FakeIRegisterStreams : IRegisterStreams
        {
            public IFlushEvents ProvidedEvents { get; private set; }
            public object ProvidedId { get; private set; }

            public void Register<TId, TAggregate>(TId id, TAggregate aggregate) where TAggregate : IFlushEvents
            {
                ProvidedId = id;
                ProvidedEvents = aggregate;
            }
        }

        private class AggregateUnderTest : Aggregate<object>
        {
            public AggregateUnderTest(object id) : base(id)
            {
            }
        }

        private object _id;
        private ICanBeRegistered _target;
        private AggregateUnderTest _aggregate;
        private FakeIRegisterStreams _registerer;
    }
}