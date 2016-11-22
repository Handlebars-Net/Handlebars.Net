using NUnit.Framework;

namespace HandlebarsDotNet.Test
{
    [TestFixture]
    public class ExceptionTests
    {
        [Test]
        [ExpectedException("HandlebarsDotNet.HandlebarsCompilerException", ExpectedMessage = "Reached end of template before block expression 'if' was closed")]
        public void TestNonClosingBlockExpressionException()
        {
            Handlebars.Compile( "{{#if 0}}test" )( new { } );
        }
    }
}
