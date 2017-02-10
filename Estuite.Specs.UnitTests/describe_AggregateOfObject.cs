using System;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_Aggregate")]
    public class describe_AggregateOfObject : nspec
    {
        private void before_each()
        {
            _id = new object();
        }

        private void when_create()
        {
            act = () => _target = new AggregateUnderTest(_id);
            it["is identified with id"] = () => _target.ProvidedId.ShouldBe(_id);
            context["and id is empty"] = () =>
            {
                before = () => _id = null;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Can't create an aggregate with id as null or default value.\r\nParameter name: id"
                );
            };
        }

        private class AggregateUnderTest : Aggregate<object>
        {
            public AggregateUnderTest(object id) : base(id)
            {
            }

            public object ProvidedId => Id;
        }

        private object _id;
        private AggregateUnderTest _target;
    }
}