using System;
using System.IO;
using Xunit;

namespace HandlebarsDotNet.Test
{
    /// <summary>
    /// Regression tests for GitHub issue #581:
    /// Handlebars.Net fails silently in .NET 8 Windows Service context.
    /// Templates produced no output and threw no exceptions.
    /// Root cause: broad exception swallowing in observable collection publish paths
    /// could prevent helper/template registrations from propagating in restricted
    /// runtime environments, causing silent empty output.
    /// </summary>
    public class Issue581Tests
    {
        [Fact]
        public void BasicCompileAndRender_NeverSilentlyFails()
        {
            var h = Handlebars.Create();
            var template = h.Compile("Hello {{name}}!");
            var result = template(new { name = "World" });
            Assert.Equal("Hello World!", result);
        }

        [Fact]
        public void CompileWithTextReader_ProducesOutput()
        {
            var h = Handlebars.Create();
            using var reader = new StringReader("Hello {{name}}!");
            var template = h.Compile(reader);
            using var writer = new StringWriter();
            template(writer, new { name = "World" }, null);
            Assert.Equal("Hello World!", writer.ToString());
        }

        [Fact]
        public void RegisteredHelper_IsInvokedDuringRender()
        {
            var h = Handlebars.Create();
            h.RegisterHelper("greet", (writer, context, arguments) =>
            {
                writer.WriteSafeString($"Hello {arguments[0]}");
            });

            var template = h.Compile("{{greet name}}");
            var result = template(new { name = "World" });
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void RegisteredTemplate_IsResolvedAsPartial()
        {
            var h = Handlebars.Create();
            h.RegisterTemplate("greeting", "Hello {{name}}");

            var template = h.Compile("{{> greeting}}");
            var result = template(new { name = "World" });
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void MultipleProperties_AreAllRendered()
        {
            var h = Handlebars.Create();
            var template = h.Compile("{{first}} {{last}}");
            var result = template(new { first = "John", last = "Doe" });
            Assert.Equal("John Doe", result);
        }

        [Fact]
        public void NestedObject_PropertyAccess_Succeeds()
        {
            var h = Handlebars.Create();
            var template = h.Compile("{{person.name}}");
            var result = template(new { person = new { name = "World" } });
            Assert.Equal("World", result);
        }

        [Fact]
        public void BlockHelper_IfElse_ProducesOutput()
        {
            var h = Handlebars.Create();
            var template = h.Compile("{{#if show}}yes{{else}}no{{/if}}");
            Assert.Equal("yes", template(new { show = true }));
            Assert.Equal("no", template(new { show = false }));
        }

        [Fact]
        public void SharedEnvironment_CompileAndRender_NeverSilentlyFails()
        {
            var h = Handlebars.Create();
            var shared = h.CreateSharedEnvironment();

            var template = shared.Compile("Hello {{name}}!");
            var result = template(new { name = "World" });
            Assert.Equal("Hello World!", result);
        }

        [Fact]
        public void EmptyTemplate_ProducesEmptyString_NotNull()
        {
            var h = Handlebars.Create();
            var template = h.Compile("");
            var result = template(new { });
            // An empty template should produce an empty string, not null
            Assert.NotNull(result);
            Assert.Equal("", result);
        }

        [Fact]
        public void StaticText_Template_ProducesOutput()
        {
            var h = Handlebars.Create();
            var template = h.Compile("static text only");
            var result = template(new { });
            Assert.Equal("static text only", result);
        }
    }
}
