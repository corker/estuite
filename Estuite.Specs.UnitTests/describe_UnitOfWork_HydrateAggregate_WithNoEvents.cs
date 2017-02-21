using System;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using Estuite.StreamStore;
using NSpec;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_UnitOfWork")]
    public class describe_UnitOfWork_HydrateAggregate_WithNoEvents : nspec
    {
        private void before_each()
        {
            _bucketId = new BucketId("bucket-id");
            _aggregate = new FakeAggregate(Guid.NewGuid());
            _readStreams = new FakeIReadStreams();
            _target = new UnitOfWork(_bucketId, _readStreams, null, null);
        }

        private void when_hydrate()
        {
            actAsync = async () => await _target.Hydrate(_aggregate, CancellationToken.None);
            it["throws exception"] = expect<StreamNotFoundException>();
        }

        private void when_try_hydrate()
        {
            actAsync = async () => _returns = await _target.TryHydrate(_aggregate, CancellationToken.None);
            it["returns false"] = () => _returns.ShouldBeFalse();
        }

        private class FakeAggregate : Aggregate<Guid>
        {
            public FakeAggregate(Guid id) : base(id)
            {
            }
        }

        private class FakeIReadStreams : IReadStreams
        {
            public async Task Read(
                StreamId streamId,
                IHydrateEvents events,
                CancellationToken token = new CancellationToken())
            {
                throw new StreamNotFoundException();
            }

            public async Task<bool> TryRead(
                StreamId streamId,
                IHydrateEvents events,
                CancellationToken token = new CancellationToken())
            {
                return false;
            }
        }

        private UnitOfWork _target;
        private BucketId _bucketId;
        private FakeAggregate _aggregate;
        private FakeIReadStreams _readStreams;
        private bool _returns;
    }
}