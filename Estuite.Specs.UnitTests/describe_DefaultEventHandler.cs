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
            _aggregate = new FakeAggregate();
            _target = new DefaultEventHandler();
        }

        private void when_handle()
        {
            act = () => _target.Handle(_aggregate, new FakeEventWithHandler());
            it["should execute handler"] = () => _aggregate.Counter.ShouldBe(1);
        }

        private void when_handle_multiple_times()
        {
            before = () =>
            {
                var @event = new FakeEventWithHandler();
                var aggregate = new FakeAggregate();
                var target = new DefaultEventHandlerWithNoCache();
                var stopwatch = Stopwatch.StartNew();
                for (var i = 0; i < Times; i++) target.Handle(aggregate, @event);
                _elapsedNoCache = stopwatch.Elapsed;
            };
            act = () =>
            {
                var @event = new FakeEventWithHandler();
                var stopwatch = Stopwatch.StartNew();
                for (var i = 0; i < Times; i++) _target.Handle(_aggregate, @event);
                _elapsed = stopwatch.Elapsed;
            };
            it["executes handler multiple times"] = () => _aggregate.Counter.ShouldBe(Times);
            it["is faster than with no cache"] = () => _elapsed.ShouldBeLessThan(_elapsedNoCache);
        }

        private FakeAggregate _aggregate;
        private DefaultEventHandler _target;
        private TimeSpan _elapsed;
        private TimeSpan _elapsedNoCache;
        private const int Times = 100000;

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
    }
}