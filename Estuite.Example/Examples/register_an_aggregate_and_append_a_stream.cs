using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Estuite.Example.Domain.Aggregates;
using Estuite.StreamStore;

namespace Estuite.Example.Examples
{
    public class register_an_aggregate_and_append_a_stream : IRunExamples
    {
        private readonly ILifetimeScope _scope;

        public register_an_aggregate_and_append_a_stream(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public async Task Run()
        {
            //var accountId = Guid.NewGuid();

            //using (var scope = _scope.BeginLifetimeScope())
            //{
            //    var uow = scope.Resolve<UnitOfWork>();
            //    var aggregate = Account.Register(accountId, "MyAccount1");
            //    uow.Register(aggregate);
            //    await uow.Commit(CancellationToken.None);
            //    aggregate.ChangeName("MyAccount2");
            //    await uow.Commit(CancellationToken.None);
            //}

            //using (var scope = _scope.BeginLifetimeScope())
            //{
            //    var uow = scope.Resolve<UnitOfWork>();
            //    var aggregate = new Account(accountId);
            //    await uow.Hydrate(aggregate, CancellationToken.None);
            //}
        }
    }
}