using System;
using Estuite.Domain;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_DefaultStreamIdentityFactory : nspec
    {
        private void before_each()
        {
            _target = new DefaultStreamIdentityFactory();
            _bucketId = new BucketId("bucket-id");
            _id = "aggregate-id";
        }

        private void when_create()
        {
            act = () => _streamId = _target.Create(_bucketId, _id, typeof(Aggregate<string>));
            it["has value"] = () => { _streamId.Value.ShouldBe(ExpectedValue); };
            context["and bucket id is null"] = () =>
            {
                before = () => _bucketId = null;
                it["throws exception"] =
                    expect<ArgumentNullException>("Value cannot be null.\r\nParameter name: bucketId");
            };
            context["and aggregate id is null"] = () =>
            {
                before = () => _id = null;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: id"
                );
            };
        }

        private DefaultStreamIdentityFactory _target;
        private BucketId _bucketId;
        private string _id;
        private StreamId _streamId;
        private static readonly string ExpectedValue = "bucket-id^Aggregate`1^aggregate-id";
    }
}