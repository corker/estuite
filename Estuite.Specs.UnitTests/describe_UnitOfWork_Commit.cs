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
    public class describe_UnitOfWork_Commit : nspec
    {
        private void before_each()
        {
            var bucketId = new BucketId("bucket-id");
            _readStreams = new FakeIReadStreams();
            _writeStreams = new FakeIWriteStreams();
            _aggregates = new UnitOfWork(bucketId, _readStreams, _writeStreams);
            _target = (ICommitAggregates) _aggregates;
        }

        private void when_commit()
        {
            actAsync = async () => await _target.Commit();
            it["sends no events to the writer"] = () => _writeStreams.Records.ShouldBeEmpty();
            context["when has aggregates"] = () =>
            {
                beforeAsync = async () => await _aggregates.Get<FakeAggregate>(1);
                it["sends events to the writer"] = () => _writeStreams.Records.Count.ShouldBe(1);
                context["and multiple aggregates have changes"] = () =>
                {
                    beforeAsync = async () => await _aggregates.Get<FakeAggregate>(2);
                    it["throws exception"] = expect<CommitMultipleStreamsException>(
                        @"Can't commit changes from multiple streams.
StreamId bucket-id^FakeAggregate^1 with 1 event(s)
StreamId bucket-id^FakeAggregate^2 with 1 event(s)"
                    );
                };
            };
        }

        private ICommitAggregates _target;
        private IReadStreams _readStreams;
        private FakeIWriteStreams _writeStreams;
        private IProvideAggregates _aggregates;

        private class FakeIReadStreams : IReadStreams
        {
            private readonly IEnumerable<EventRecord> _events;

            public FakeIReadStreams()
            {
                _events = new List<EventRecord> {new EventRecord(1, "")};
            }

            public async Task Read(
                StreamId streamId,
                IReceiveEventRecords records,
                CancellationToken token
            )
            {
                records.Receive(_events);
            }
        }

        private class FakeAggregate : IReceiveEvents, ISendEvents
        {
            protected FakeAggregate(int id)
            {
            }

            public void Receive(IEnumerable<Event> events)
            {
            }

            public void SendTo(IReceiveEvents receiver)
            {
                var events = new List<Event> {new Event(1, new object())};
                receiver.Receive(events);
            }
        }

        private class FakeIWriteStreams : IWriteStreams
        {
            public FakeIWriteStreams()
            {
                Records = new List<EventRecord>();
            }

            public List<EventRecord> Records { get; }

            public async Task Write(
                StreamId streamId,
                IReadOnlyCollection<EventRecord> records,
                CancellationToken token
            )
            {
                Records.AddRange(records);
            }
        }
    }
}