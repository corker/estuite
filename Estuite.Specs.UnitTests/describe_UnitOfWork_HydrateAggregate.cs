using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_UnitOfWork")]
    public class describe_UnitOfWork_HydrateAggregate : nspec
    {
        private void before_each()
        {
            _bucketId = new BucketId("bucket-id");
            _aggregate = new FakeICanBeHydrated();
            _target = new UnitOfWork(_bucketId, null, null, null);
        }

        private void when_register_aggregate()
        {
            act = () => _target.Hydrate(_aggregate);
            it["calls aggregate with itself"] = () => _aggregate.HydratedTo.ShouldBeSameAs(_target);
        }

        private class FakeICanBeHydrated : ICanBeHydrated
        {
            public IHydrateEventStreams HydratedTo { get; private set; }

            public async Task HydrateTo(IHydrateEventStreams streams, CancellationToken token = new CancellationToken())
            {
                HydratedTo = streams;
            }
        }

        private UnitOfWork _target;
        private BucketId _bucketId;
        private FakeICanBeHydrated _aggregate;
    }
}