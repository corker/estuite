using System;
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
            var accountId = Guid.NewGuid();

            using (var scope = _scope.BeginLifetimeScope())
            {
                var aggregates = scope.Resolve<IProvideAggregates>();
                var uow = scope.Resolve<ICommitAggregates>();
                var aggregate = await aggregates.Get<Account>(accountId);
                aggregate.Register("MyAccount1");
                await uow.Commit();
                aggregate.ChangeName("MyAccount2");
                await uow.Commit();
            }

            using (var scope = _scope.BeginLifetimeScope())
            {
                var aggregates = scope.Resolve<IProvideAggregates>();
                var aggregate = await aggregates.Get<Account>(accountId);
            }
        }
    }
}