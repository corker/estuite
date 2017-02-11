using System;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_SerializedEvent : nspec
    {
        private void before_each()
        {
            _type = "event-type";
            _payoad = "event-payload";
        }

        private void when_create()
        {
            act = () => _event = new SerializedEvent(_type, _payoad);
            it["has type"] = () => { _event.Type.ShouldBe(_type); };
            it["has payload"] = () => { _event.Payload.ShouldBe(_payoad); };
            context["and type is null"] = () =>
            {
                before = () => _type = null;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: type"
                );
            };
            context["and type is empty string"] = () =>
            {
                before = () => _type = string.Empty;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: type"
                );
            };
            context["and payload is null"] = () =>
            {
                before = () => _payoad = null;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: payload"
                );
            };
            context["and payload is empty string"] = () =>
            {
                before = () => _payoad = string.Empty;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: payload"
                );
            };
        }

        private SerializedEvent _event;
        private string _type;
        private string _payoad;
        private const string ExpectedValue = "bucket-id^aggregate-type^aggregate-id";
    }
}