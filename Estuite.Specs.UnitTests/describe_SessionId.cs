using System;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_SessionId : nspec
    {
        private void before_each()
        {
            _value = "session-id";
        }

        private void when_create()
        {
            act = () => _sessionId = new SessionId(_value);
            it["has value"] = () => _sessionId.Value.ShouldBe(_value);
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
        private SessionId _sessionId;
    }
}