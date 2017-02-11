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
            act = () => _streamId = _target.Create<string, FakeAggregate>(_bucketId, _id);
            it["creates stream id with expected value"] =
                () => { _streamId.Value.ShouldBe("bucket-id^FakeAggregate^aggregate-id"); };
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

        // ReSharper disable once ClassNeverInstantiated.Local
        private class FakeAggregate : Aggregate<string>
        {
            private FakeAggregate() : base(null)
            {
            }
        }

        private DefaultStreamIdentityFactory _target;
        private BucketId _bucketId;
        private string _id;
        private StreamId _streamId;
    }
}