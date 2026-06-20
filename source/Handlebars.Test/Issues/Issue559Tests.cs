using Xunit;
using Xunit.Abstractions;

namespace HandlebarsDotNet.Test
{
    public class Issue559Tests
    {
        private readonly ITestOutputHelper _output;

        public Issue559Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void WriteSafeString_ConsistentRegardlessOfRegistrationOrder()
        {
            HandlebarsHelper link_to = (writer, context, parameters) =>
                writer.WriteSafeString($"<a href='{context["url"]}'>{context["text"]}</a>");

            string source = "Click here: {{link_to}}";
            var data = new { url = "https://example.com", text = "Click" };

            // Register BEFORE compile
            var h1 = Handlebars.Create();
            h1.RegisterHelper("link_to", link_to);
            var t1 = h1.Compile(source);
            var result1 = t1(data);

            // Register AFTER compile
            var h2 = Handlebars.Create();
            var t2 = h2.Compile(source);
            h2.RegisterHelper("link_to", link_to);
            var result2 = t2(data);

            _output.WriteLine($"result1 (before compile): {result1}");
            _output.WriteLine($"result2 (after compile): {result2}");

            // Both should produce identical, unescaped HTML
            Assert.Equal(result1, result2);
            Assert.Contains("<a href=", result1);
        }
    }
}
