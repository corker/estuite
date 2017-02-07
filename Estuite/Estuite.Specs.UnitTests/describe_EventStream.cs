using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_Aggregate : nspec
    {
        private void when_()
        {
            it["should be true"] = () => 0.ShouldBe(0);
        }
    }
}