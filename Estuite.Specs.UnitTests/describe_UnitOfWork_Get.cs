using System;
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
    public class describe_UnitOfWork_Get : nspec
    {
        private void before_each()
        {
            var bucketId = new BucketId("bucket-id");
            _expectedEvents = new List<Event>
            {
                new Event(1, "")
            };
            _readStreams = new FakeIReadStreams(new List<EventRecord>
            {
                new EventRecord(1, "")
            });
            _target = new UnitOfWork(bucketId, _readStreams, null);
            _id = 1;
        }

        private void when_get()
        {
            actAsync = async () => _receiver = await _target.Get<FakeIReceiveEvents>(_id);
            it["returns a receiver"] = () => _receiver.ShouldNotBeNull();
            it["returns a receiver with expected id"] = () => _receiver.Id.ShouldBe(_id);
            it["sends events to the receiver"] = () => _receiver.Events.Count.ShouldBe(1);
            context["and id is null"] = () =>
            {
                before = () => _id = null;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: id"
                );
            };
            context["and id is zero"] = () =>
            {
                before = () => _id = 0;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: id"
                );
            };
            context["and id is empty string"] = () =>
            {
                before = () => _id = string.Empty;
                it["throws exception"] = expect<ArgumentOutOfRangeException>(
                    "Specified argument was out of the range of valid values.\r\nParameter name: id"
                );
            };
        }

        private IProvideAggregates _target;
        private IReadStreams _readStreams;
        private object _id;
        private FakeIReceiveEvents _receiver;
        private List<Event> _expectedEvents;

        private class FakeIReadStreams : IReadStreams
        {
            private readonly IEnumerable<EventRecord> _events;

            public FakeIReadStreams(IEnumerable<EventRecord> events)
            {
                _events = events;
            }

            public async Task Read(
                StreamId streamId,
                IReceiveEventRecords records,
                CancellationToken token
            )
            {
                records.Receive(_events);
            }

            public async Task<bool> TryRead(
                StreamId streamId,
                IReceiveEventRecords records,
                CancellationToken token
            )
            {
                throw new NotImplementedException();
            }
        }

        private class FakeIReceiveEvents : IReceiveEvents
        {
            protected FakeIReceiveEvents(int id)
            {
                Id = id;
                Events = new List<Event>();
            }

            public int Id { get; }

            public List<Event> Events { get; }

            public void Receive(IEnumerable<Event> events)
            {
                Events.AddRange(events);
            }
        }
    }
}