using Shouldly;

namespace Estuite.Specs.UnitTests
{
    public class describe_Dummy : nspec
    {
        private void when_something()
        {
            it["should be true"] = () => 0.ShouldBe(0);
        }
    }
}