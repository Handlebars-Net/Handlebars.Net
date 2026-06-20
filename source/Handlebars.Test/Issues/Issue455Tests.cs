using Xunit;

namespace HandlebarsDotNet.Test
{
    public class Issue455Tests
    {
        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/455
        [Fact]
        public void Issue455_NamedArgsInPartialInsideNestedEach()
        {
            var h = Handlebars.Create();
            h.RegisterTemplate("myPartial", "{{arg1}}-{{arg2}} ");
            var template = h.Compile(
                "{{#each items}}{{#each nested}}{{> myPartial arg1=value1 arg2=value2}}{{/each}}{{/each}}");
            var data = new
            {
                items = new[] { new { nested = new[] { new { value1 = "A", value2 = "B" }, new { value1 = "C", value2 = "D" } } } }
            };
            var result = template(data);
            Assert.Contains("A-B", result);
            Assert.Contains("C-D", result);
        }
    }
}
