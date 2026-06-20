using Xunit;

namespace HandlebarsDotNet.Test
{
    public class Issue458Tests
    {
        [Fact]
        public void Issue458_BasicCompileAndRender_NoByRefDelegate()
        {
            // Validates the scenario that fails on Mono when byref delegates are used
            var h = Handlebars.Create();
            var render = h.Compile("{{input}}");
            var result = render(new { input = 42 });
            Assert.Equal("42", result);
        }

        [Fact]
        public void Issue458_BlockHelper_NoByRefDelegate()
        {
            // Block helpers also exercise TemplateDelegate compilation
            var h = Handlebars.Create();
            h.RegisterHelper("loud", (writer, options, context, arguments) =>
            {
                options.Template(writer, context);
            });
            var render = h.Compile("{{#loud}}hello{{/loud}}");
            var result = render(new { });
            Assert.Equal("hello", result);
        }

        [Fact]
        public void Issue458_NestedTemplates_NoByRefDelegate()
        {
            // Nested template compilation exercises the expression tree lambda paths
            var h = Handlebars.Create();
            var render = h.Compile("{{#each items}}{{this}},{{/each}}");
            var result = render(new { items = new[] { "a", "b", "c" } });
            Assert.Equal("a,b,c,", result);
        }
    }
}
