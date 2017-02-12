using System;
using System.Threading;
using System.Threading.Tasks;
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
            _eventStreams = new FakeIHydrateEventStreams();
        }

        private void when_hydrate()
        {
            actAsync = async () => await _target.HydrateTo(_eventStreams);
            it["provides an id with expected type"] = () => { _eventStreams.ProvidedId.ShouldBeOfType<object>(); };
            it["provides an expected id"] = () => { _eventStreams.ProvidedId.ShouldBe(_id); };
            it["provides itself to hydrate events"] =
                () => { _eventStreams.ProvidedEvents.ShouldBeSameAs(_aggregate); };
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

            public async Task Hydrate<TId, TEventStream>(
                TId id,
                TEventStream stream,
                CancellationToken token = new CancellationToken())
                where TEventStream : IHydrateEvents, IFlushEvents
            {
                ProvidedId = id;
                ProvidedEvents = stream;
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
        private FakeIHydrateEventStreams _eventStreams;
    }
}