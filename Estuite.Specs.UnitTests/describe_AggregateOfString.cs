using System;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_Aggregate")]
    public class describe_AggregateOfString : nspec
    {
        private void before_each()
        {
            _id = "A";
        }

        private void when_create()
        {
            act = () => _target = new AggregateUnderTest(_id);
            it["is identified with id"] = () => _target.ProvidedId.ShouldBe(_id);
            context["and id is empty"] = () =>
            {
                before = () => _id = null;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: id"
                );
            };
        }

        private class AggregateUnderTest : Aggregate<string>
        {
            public AggregateUnderTest(string id) : base(id)
            {
            }

            public string ProvidedId => Id;
        }

        private string _id;
        private AggregateUnderTest _target;
    }
}