using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Estuite.Domain;
using Estuite.Example.Domain.Events;
using Estuite.StreamStore;

namespace Estuite.Example.Examples
{
    public class session_collision : IRunExamples
    {
        //private readonly BucketId _bucketId;
        //private readonly ICreateSessions _sessions;
        //private readonly IWriteStreams _streams;

        //public session_collision(BucketId bucketId, ICreateSessions sessions, IWriteStreams streams)
        //{
        //    _bucketId = bucketId;
        //    _sessions = sessions;
        //    _streams = streams;
        //}

        public async Task Run()
        {
            //var accountId = Guid.NewGuid();
            //var sessionId = new SessionId($"{Guid.NewGuid()}");
            //var aggregateType = new AggregateType("Account");
            //var aggregateId = new AggregateId($"{Guid.NewGuid()}");

            //var streamId = new StreamId(_bucketId, aggregateType, aggregateId);
            //var session = _sessions.Create(
            //    streamId,
            //    sessionId,
            //    new[]
            //    {
            //        new Event(1, new AccountRegistered {AccountId = accountId, Name = "MyAccount4"})
            //    });
            //await _streams.Write(session);

            //session = _sessions.Create(
            //    streamId,
            //    sessionId,
            //    new[]
            //    {
            //        new Event(2, new AccountNameCorrected {AccountId = accountId, Name = "MyAccount4"})
            //    });
            //try
            //{
            //    await _streams.Write(session);
            //}
            //catch (StreamConcurrentWriteException e)
            //{
            //    Debug.WriteLine($"{e}");
            //    return;
            //}
            //throw new Exception("Something went wrong...");
        }
    }
}