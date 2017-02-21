using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Estuite.Example.Domain.Aggregates;
using Estuite.StreamStore;

namespace Estuite.Example.Examples
{
    public class version_collision : IRunExamples
    {
        private readonly ILifetimeScope _scope;

        public version_collision(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public async Task Run()
        {
            var accountId = Guid.NewGuid();

            using (var scope = _scope.BeginLifetimeScope())
            {
                var uow = scope.Resolve<UnitOfWork>();
                var aggregate = Account.Register(accountId, "MyAccount3");
                uow.Register(aggregate);
                await uow.Commit(CancellationToken.None);
            }

            using (var scope = _scope.BeginLifetimeScope())
            {
                var uow = scope.Resolve<UnitOfWork>();
                var aggregate = Account.Register(accountId, "MyAccount3");
                uow.Register(aggregate);
                try
                {
                    await uow.Commit(CancellationToken.None);
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
}