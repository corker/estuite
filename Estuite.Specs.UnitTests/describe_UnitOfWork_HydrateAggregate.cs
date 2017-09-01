using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using Estuite.StreamStore;
using NSpec;
using Shouldly;
using IReadStreams = Estuite.Domain.IReadStreams;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_UnitOfWork")]
    public class describe_UnitOfWork_HydrateAggregate : nspec
    {
        private void before_each()
        {
            _bucketId = new BucketId("bucket-id");
            _aggregate = new FakeICanReadStreams();
            _target = new UnitOfWork(_bucketId, null, null, null);
        }

        private void when_hydrate_aggregate()
        {
            actAsync = async () => await _target.Hydrate(_aggregate, CancellationToken.None);
            it["calls aggregate with itself"] = () => _aggregate.HydratedTo.ShouldBeSameAs(_target);
        }

        private void when_try_hydrate_aggregate()
        {
            actAsync = async () => await _target.TryHydrate(_aggregate, CancellationToken.None);
            it["calls aggregate with itself"] = () => _aggregate.TryHydratedTo.ShouldBeSameAs(_target);
        }

        private class FakeICanReadStreams : ICanReadStreams
        {
            public IReadStreams HydratedTo { get; private set; }

            public IReadStreams TryHydratedTo { get; private set; }

            public async Task ReadFrom(IReadStreams streams, CancellationToken token)
            {
                HydratedTo = streams;
            }
            
            public async Task<bool> TryReadFrom(IReadStreams streams, CancellationToken token)
            {
                TryHydratedTo = streams;
                return true;
            }
        }

        private UnitOfWork _target;
        private BucketId _bucketId;
        private FakeICanReadStreams _aggregate;
    }
}