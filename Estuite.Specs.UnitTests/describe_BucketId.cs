using System;
using Estuite.StreamStore;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_BucketId : nspec
    {
        private void before_each()
        {
            _value = "bucket-id";
        }

        private void when_create()
        {
            act = () => _bucketId = new BucketId(_value);
            it["has value"] = () => _bucketId.Value.ShouldBe(_value);
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
        private BucketId _bucketId;
    }
}