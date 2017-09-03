using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Estuite.Domain;
using Estuite.StreamStore;
using NSpec;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [Tag("describe_UnitOfWork")]
    public class describe_UnitOfWork_Get_Async : nspec
    {
        private void before_each()
        {
            var bucketId = new BucketId("bucket-id");
            _readStreams = new FakeIReadStreams();
            _target = new UnitOfWork(bucketId, _readStreams, null);
            _readStreams.SetTarget(_target);
        }

        private void when_get_called_twice_asynchronousely()
        {
            actAsync = async () => _receiver = await _target.Get<FakeIReceiveEvents>(1);
            it["returns the same receiver"] = () => _receiver.ShouldBeSameAs(_readStreams.Receiver);
        }

        private IProvideAggregates _target;
        private FakeIReadStreams _readStreams;
        private FakeIReceiveEvents _receiver;

        private class FakeIReadStreams : IReadStreams
        {
            private IProvideAggregates _aggregates;
            private int _callsCount;

            public FakeIReceiveEvents Receiver { get; private set; }

            public async Task Read(
                StreamId streamId,
                IReceiveEventRecords records,
                CancellationToken token
            )
            {
                _callsCount++;
                if (_callsCount == 2) return;
                Receiver = await _aggregates.Get<FakeIReceiveEvents>(1, token);
            }

            public void SetTarget(IProvideAggregates aggregates)
            {
                _aggregates = aggregates;
            }
        }

        private class FakeIReceiveEvents : IReceiveEvents
        {
            protected FakeIReceiveEvents(int id)
            {
            }

            public void Receive(IEnumerable<Event> events)
            {
            }
        }
    }
}