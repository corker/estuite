using System;
using Estuite.StreamStore;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_AggregateType : nspec
    {
        private void before_each()
        {
            _value = "aggregate-type";
        }

        private void when_create()
        {
            act = () => _aggregateType = new AggregateType(_value);
            it["has value"] = () => _aggregateType.Value.ShouldBe(_value);
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
        private AggregateType _aggregateType;
    }
}