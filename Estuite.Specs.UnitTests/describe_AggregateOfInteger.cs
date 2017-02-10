using System;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_Aggregate")]
    public class describe_AggregateOfInteger : nspec
    {
        private void before_each()
        {
            _id = 1;
        }

        private void when_create()
        {
            act = () => _target = new AggregateUnderTest(_id);
            it["is identified with id"] = () => _target.ProvidedId.ShouldBe(_id);
            context["and id is empty"] = () =>
            {
                before = () => _id = 0;
                it["throws exception"] = expect<ArgumentOutOfRangeException>("Can't create an aggregate with id as null or default value.\r\nParameter name: id");
            };
        }

        private class AggregateUnderTest : Aggregate<int>
        {
            public AggregateUnderTest(int id) : base(id)
            {
            }

            public int ProvidedId => Id;
        }

        private int _id;
        private AggregateUnderTest _target;
    }
}