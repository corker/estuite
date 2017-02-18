using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using Estuite.StreamStore;
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

        private void when_hydrate_aggregate()
        {
            actAsync = async () => await _target.Hydrate(_aggregate);
            it["calls aggregate with itself"] = () => _aggregate.HydratedTo.ShouldBeSameAs(_target);
        }

        private void when_try_hydrate_aggregate()
        {
            actAsync = async () => await _target.TryHydrate(_aggregate);
            it["calls aggregate with itself"] = () => _aggregate.TryHydratedTo.ShouldBeSameAs(_target);
        }

        private class FakeICanBeHydrated : ICanBeHydrated
        {
            public IHydrateStreams HydratedTo { get; private set; }

            public IHydrateStreams TryHydratedTo { get; private set; }

            public async Task HydrateTo(IHydrateStreams streams, CancellationToken token = new CancellationToken())
            {
                HydratedTo = streams;
            }

            public async Task<bool> TryHydrateTo(IHydrateStreams streams,
                CancellationToken token = new CancellationToken())
            {
                TryHydratedTo = streams;
                return true;
            }
        }

        private UnitOfWork _target;
        private BucketId _bucketId;
        private FakeICanBeHydrated _aggregate;
    }
}