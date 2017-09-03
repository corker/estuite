using System;
using System.Threading.Tasks;
using Estuite.Example.Domain.Aggregates;
using Estuite.StreamStore;

namespace Estuite.Example.Examples
{
    public class get_aggregate_that_doesnt_exist : IRunExamples
    {
        private readonly IProvideAggregates _aggregates;

        public get_aggregate_that_doesnt_exist(IProvideAggregates aggregates)
        {
            _aggregates = aggregates;
        }

        public async Task Run()
        {
            var aggregate = await _aggregates.Get<Account>(Guid.NewGuid());
        }
    }
}