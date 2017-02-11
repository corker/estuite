using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_UnitOfWork")]
    public class describe_UnitOfWork_RegisterAggregate : nspec
    {
        private void before_each()
        {
            _bucketId = new BucketId("bucket-id");
            _aggregate = new FakeICanBeRegistered();
            _target = new UnitOfWork(_bucketId, null, null);
        }

        private void when_register_aggregate()
        {
            act = () => _target.Register(_aggregate);
            it["calls aggregate with itself"] = () => _aggregate.RegisteredTo.ShouldBeSameAs(_target);
        }

        private class FakeICanBeRegistered : ICanBeRegistered
        {
            public IRegisterEventStreams RegisteredTo { get; private set; }

            public void RegisterTo(IRegisterEventStreams streams)
            {
                RegisteredTo = streams;
            }
        }

        private UnitOfWork _target;
        private BucketId _bucketId;
        private FakeICanBeRegistered _aggregate;
    }
}