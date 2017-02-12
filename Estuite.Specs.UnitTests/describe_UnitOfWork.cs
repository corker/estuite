using System;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_UnitOfWork : nspec
    {
        private void before_each()
        {
            _bucketId = new BucketId("bucket-id");
        }

        private void when_create()
        {
            act = () => _target = new UnitOfWork(_bucketId, null, null, null);
            it["has been created"] = () => _target.ShouldNotBeNull();
            context["and bucket id is null"] = () =>
            {
                before = () => _bucketId = null;
                it["throws exception"] = expect<ArgumentNullException>(
                    "Value cannot be null.\r\nParameter name: bucketId"
                );
            };
        }

        private UnitOfWork _target;
        private BucketId _bucketId;
    }
}