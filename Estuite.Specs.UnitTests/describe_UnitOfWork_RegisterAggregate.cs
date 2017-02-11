using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
            _createSessions = new FakeICreateSessions();
            _writeSessions = new FakeIWriteSessions();
            _aggregate = new FakeICanBeRegistered();
            _target = new UnitOfWork(_bucketId, _createSessions, _writeSessions);
        }

        private void when_register_aggregate()
        {
            act = () => _target.Register(_aggregate);
            it["calls aggregate with itself"] = () => _aggregate.RegisteredTo.ShouldBeSameAs(_target);
        }

        private class FakeIWriteSessions : IWriteSessions
        {
            public Task Write(Session session, CancellationToken token = new CancellationToken())
            {
                throw new NotImplementedException();
            }
        }

        private class FakeICreateSessions : ICreateSessions
        {
            public Session Create(StreamId streamId, SessionId sessionId, IEnumerable<Event> @event)
            {
                throw new NotImplementedException();
            }
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
        private ICreateSessions _createSessions;
        private IWriteSessions _writeSessions;
        private FakeICanBeRegistered _aggregate;
    }
}