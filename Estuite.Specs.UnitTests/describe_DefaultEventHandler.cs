using System;
using System.Diagnostics;
using Estuite.Domain;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_DefaultEventHandler : nspec
    {
        private void before_each()
        {
            _aggregate = new AggregateUnderTest();
            _target = new DefaultEventHandler();
        }

        private void when_handle()
        {
            act = () => _target.Handle(_aggregate, new EventWithHandler());
            it["should execute handler"] = () => _aggregate.Counter.ShouldBe(1);
        }

        private void when_handle_multiple_times()
        {
            before = () =>
            {
                var @event = new EventWithHandler();
                var aggregate = new AggregateUnderTest();
                var target = new DefaultEventHandlerWithNoCache();
                var stopwatch = Stopwatch.StartNew();
                for (var i = 0; i < Times; i++) target.Handle(aggregate, @event);
                _elapsedNoCache = stopwatch.Elapsed;
            };
            act = () =>
            {
                var @event = new EventWithHandler();
                var stopwatch = Stopwatch.StartNew();
                for (var i = 0; i < Times; i++) _target.Handle(_aggregate, @event);
                _elapsed = stopwatch.Elapsed;
            };
            it["executes handler multiple times"] = () => _aggregate.Counter.ShouldBe(Times);
            it["is faster than with no cache"] = () => _elapsed.ShouldBeLessThan(_elapsedNoCache);
        }

        private AggregateUnderTest _aggregate;
        private DefaultEventHandler _target;
        private TimeSpan _elapsed;
        private TimeSpan _elapsedNoCache;
        private const int Times = 100000;

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
    }
}