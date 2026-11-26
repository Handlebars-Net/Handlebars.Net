using System.Collections.Generic;
using Xunit;

namespace HandlebarsDotNet.Test
{
    [CollectionDefinition(nameof(BindingContextPoolTests), DisableParallelization = true)]
    public class BindingContextPoolCollection
    {
    }

    // Binding contexts are pooled process-wide and shared by every IHandlebars instance. A value one
    // render binds must never be visible to a later render, however many renders come between.
    [Collection(nameof(BindingContextPoolTests))]
    public class BindingContextPoolTests
    {
        [Fact]
        public void BlockParamBoundInOneRenderIsNotSeenByLaterRenders()
        {
            var first = Handlebars.Create().Compile("{{#each secrets as |x|}}{{/each}}");
            // `x` is read in two nested frames, so whichever pooled context holds the earlier
            // binding, it is read wherever that context lands except as the root
            var second = Handlebars.Create().Compile("{{#with this as |y|}}{{#with this as |z|}}[{{x}}]{{/with}}[{{x}}]{{/with}}");

            // A context returned at a version that is a multiple of 10 is fully cleared, so bind `x`
            // in several renders to leave it in more than one pooled context
            for (var i = 0; i < 20; i++) first(new { secrets = new[] { "from the first render" } });

            var leaked = new List<string>();
            for (var i = 0; i < 100000 && leaked.Count < 5; i++)
            {
                var output = second(new { x = "own value" });
                if (output != "[own value][own value]") leaked.Add(i + ": " + output);
            }

            Assert.Empty(leaked);
        }
    }
}
