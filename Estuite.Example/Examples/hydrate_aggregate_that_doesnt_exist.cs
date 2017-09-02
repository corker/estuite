using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Estuite.Domain;
using Estuite.Example.Domain.Aggregates;
using Estuite.StreamStore;

namespace Estuite.Example.Examples
{
    public class hydrate_aggregate_that_doesnt_exist : IRunExamples
    {
        //private readonly IHydrateAggregates _uow;

        public hydrate_aggregate_that_doesnt_exist(UnitOfWork uow)
        {
            //_uow = uow;
        }

        public async Task Run()
        {
            //var aggregate = new Account(Guid.NewGuid());
            //try
            //{
            //    await _uow.Hydrate(aggregate);
            //}
            //catch (StreamNotFoundException e)
            //{
            //    Debug.WriteLine($"{e}");
            //    return;
            //}
            //throw new Exception("Something went wrong...");
        }
    }
}