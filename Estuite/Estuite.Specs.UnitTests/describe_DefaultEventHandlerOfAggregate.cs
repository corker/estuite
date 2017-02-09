using System;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_DefaultEventHandler")]
    public class describe_DefaultEventHandlerOfAggregate : nspec
    {
        private void before_each()
        {
            _aggregate = new AggregateUnderTest();
            _target = new DefaultEventHandler<AggregateUnderTest>();
        }

        private void when_handle()
        {
            context["and event has a handle method"] = () =>
            {
                act = () => _target.Handle(_aggregate, new EventWithHandler());
                it["should execute handler"] = () => _aggregate.Counter.ShouldBe(1);
            };

            context["and event has no handle method"] = () =>
            {
                act = () => _target.Handle(_aggregate, new EventWithNoHandler());
                it["should throw"] = expect<ArgumentOutOfRangeException>();
            };
        }

        private class AggregateUnderTest
        {
            public int Counter;

            private void Handle(EventWithHandler @event)
            {
                Counter++;
            }
        }

        private class EventWithHandler
        {
        }

        private class EventWithNoHandler
        {
        }

        private DefaultEventHandler<AggregateUnderTest> _target;
        private AggregateUnderTest _aggregate;
    }
}