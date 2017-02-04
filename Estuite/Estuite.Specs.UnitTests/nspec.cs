using NSpec;
using NSpec.Domain;
using NSpec.Domain.Formatters;
using NUnit.Framework;
using System.Linq;
using Shouldly;

namespace Estuite.Specs.UnitTests
{
    [TestFixture]
    public abstract class nspec : NSpec.nspec
    {
        [Test]
        public void debug()
        {
            var currentSpec = GetType();
            var finder = new SpecFinder(new[] { currentSpec });
            var filter = new Tags().Parse(currentSpec.Name);
            var builder = new ContextBuilder(finder, filter, new DefaultConventions());
            var runner = new ContextRunner(filter, new ConsoleFormatter(), false);
            var result = runner.Run(builder.Contexts().Build());

            //assert that there aren't any failures
            result.Failures().Count().ShouldBe(0);
        }
    }
}