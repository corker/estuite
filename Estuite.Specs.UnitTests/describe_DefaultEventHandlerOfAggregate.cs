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
            _aggregate = new FakeAggregate();
            _target = new DefaultEventHandler<FakeAggregate>();
        }

        private void when_handle()
        {
            context["and event has a handle method"] = () =>
            {
                act = () => _target.Handle(_aggregate, new FakeEventWithHandler());
                it["should execute handler"] = () => _aggregate.Counter.ShouldBe(1);
            };

            context["and event has no handle method"] = () =>
            {
                act = () => _target.Handle(_aggregate, new FakeEventWithNoHandler());
                it["should throw"] = expect<ArgumentOutOfRangeException>();
            };
        }

        private class FakeAggregate
        {
            public int Counter;

            private void Handle(FakeEventWithHandler @event)
            {
                Counter++;
            }
        }

        private class FakeEventWithHandler
        {
        }

        private class FakeEventWithNoHandler
        {
        }

        private DefaultEventHandler<FakeAggregate> _target;
        private FakeAggregate _aggregate;
    }
}