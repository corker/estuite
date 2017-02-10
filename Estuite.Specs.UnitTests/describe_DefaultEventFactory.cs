﻿using Estuite.Domain;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_DefaultEventFactory : nspec
    {
        private DefaultEventFactory _target;
        private object _event;

        private void before_each()
        {
            _target = new DefaultEventFactory();
        }

        private void when_create()
        {
            act = () => _event = _target.Create<EventUnderTest>();
            it["returns event of type"] = () => { _event.ShouldBeOfType<EventUnderTest>(); };
        }

        private class EventUnderTest
        {
        }
    }
}