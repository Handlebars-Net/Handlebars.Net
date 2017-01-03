using NUnit.Framework;

namespace HandlebarsDotNet.Test
{
    [TestFixture]
    public class ExceptionTests
    {
        [Test]
        public void TestNonClosingBlockExpressionException()
        {
            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("{{#if 0}}test")(new { });
            },
            "Reached end of template before block expression 'if' was closed");
        }
    }
}
