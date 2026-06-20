using Xunit;

namespace HandlebarsDotNet.Test
{
    public class Issue543Tests
    {
        [Fact]
        public void Subexpression_WriteSafeString_NotDoubleEncoded()
        {
            var h = Handlebars.Create();
            h.RegisterHelper("func2", (writer, ctx, args) => writer.WriteSafeString("<b>bold</b>"));
            h.RegisterHelper("func1", (writer, ctx, args) => writer.WriteSafeString($"<span>{args[0]}</span>"));
            var result = h.Compile("{{func1 (func2)}}")(new { });
            Assert.Equal("<span><b>bold</b></span>", result);
        }

        [Fact]
        public void Standalone_WriteSafeString_NotEncoded()
        {
            var h = Handlebars.Create();
            h.RegisterHelper("func2", (writer, ctx, args) => writer.WriteSafeString("<b>bold</b>"));
            var result = h.Compile("{{func2}}")(new { });
            Assert.Equal("<b>bold</b>", result);
        }
    }
}
