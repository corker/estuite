using System;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_StreamId : nspec
    {
        private void before_each()
        {
            _bucketId = new BucketId("bucket-id");
            _aggregateType = new AggregateType("aggregate-type");
            _aggregateId = new AggregateId("aggregate-id");
        }

        private void when_create()
        {
            act = () => _streamId = new StreamId(_bucketId, _aggregateType, _aggregateId);
            it["has expected value"] = () => { _streamId.Value.ShouldBe(ExpectedValue); };
            context["and bucket id is null"] = () =>
            {
                before = () => _bucketId = null;
                it["throws exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: bucketId"
                );
            };
            context["and aggregate type is null"] = () =>
            {
                before = () => _aggregateType = null;
                it["throws exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: aggregateType"
                );
            };
            context["and aggregate id is null"] = () =>
            {
                before = () => _aggregateId = null;
                it["throws exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: aggregateId"
                );
            };
        }

        private BucketId _bucketId;
        private AggregateType _aggregateType;
        private AggregateId _aggregateId;
        private StreamId _streamId;
        private const string ExpectedValue = "bucket-id^aggregate-type^aggregate-id";
    }
}