using System;
using Estuite.Domain;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_Event : nspec
    {
        private void when_create_an_event()
        {
            before = () =>
            {
                _version = 5;
                _body = new object();
            };
            act = () => _target = new Event(_version, _body);
            it["returns an event with version"] = () => _target.Version.ShouldBe(_version);
            it["returns an event with body"] = () => _target.Body.ShouldBeSameAs(_body);

            context["and version is empty"] = () =>
            {
                before = () => _version = 0;
                it["throws an exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: version"
                );
            };

            context["and version is negative"] = () =>
            {
                before = () => _version = -100;
                it["throws an exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: version"
                );
            };

            context["and body is empty"] = () =>
            {
                before = () => _body = null;
                it["throws an exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: body"
                );
            };
        }

        private Event _target;
        private int _version;
        private object _body;
    }
}