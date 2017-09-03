using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Estuite.Example.Domain.Events;
using Estuite.StreamStore;

namespace Estuite.Example.Examples
{
    public class session_collision : IRunExamples
    {
        private readonly BucketId _bucketId;
        private readonly IWriteStreams _streams;

        public session_collision(BucketId bucketId, IWriteStreams streams)
        {
            _bucketId = bucketId;
            _streams = streams;
        }

        public async Task Run()
        {
            var accountId = Guid.NewGuid();
            var aggregateType = new AggregateType("Account");
            var aggregateId = new AggregateId($"{Guid.NewGuid()}");

            var streamId = new StreamId(_bucketId, aggregateType, aggregateId);
            var records = new List<EventRecord>
            {
                new EventRecord(1, new AccountRegistered {AccountId = accountId, Name = "MyAccount4"})
            };
            await _streams.Write(streamId, records);

            records = new List<EventRecord>
            {
                new EventRecord(2, new AccountNameCorrected {AccountId = accountId, Name = "MyAccount4"})
            };
            try
            {
                await _streams.Write(streamId, records);
            }
            catch (StreamConcurrentWriteException e)
            {
                Debug.WriteLine($"{e}");
                return;
            }
            throw new Exception("Something went wrong...");
        }
    }
}