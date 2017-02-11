using System;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_AggregateId : nspec
    {
        private void before_each()
        {
            _value = "aggregate-id";
        }

        private void when_create()
        {
            act = () => _aggregateId = new AggregateId(_value);
            it["has value"] = () => _aggregateId.Value.ShouldBe(_value);
            context["and value is null"] = () =>
            {
                before = () => _value = null;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: value"
                );
            };
            context["and value is empty string"] = () =>
            {
                before = () => _value = string.Empty;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: value"
                );
            };
        }

        private string _value;
        private AggregateId _aggregateId;
    }
}