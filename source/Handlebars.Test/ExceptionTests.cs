using Xunit;

namespace HandlebarsDotNet.Test
{
    public class ExceptionTests
    {
        [Fact]
        public void TestNonClosingBlockExpressionException()
        {
            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("{{#if 0}}test")(new { });
            });
        }
        
        [Fact]
        public void TestLooseClosingBlockExpressionException()
        {
            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("{{#if 0}}test{{/if}}{{/unless}}")(new { });
            });
        }
        
        [Fact]
        public void TestNestedLooseClosingBlockExpressionException()
        {
            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("{{#if 1}}{{#unless 0}}test{{/if}}{{/unless}}{{/if}}")(new { });
            });
        }

        [Fact]
        public void TestUnmatchedClosingBlockExpressionException()
        {
            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("{{#if 0}}test{{/unless}}")(new { });
            });
        }
        
        [Fact]
        public void TestLooseClosingBlockInIteratorExpressionException()
        {
            var data = new
                {
                    enumerateMe = new
                        {
                            foo = "hello",
                            bar = "world"
                        }
                };
            
            Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("{{#each enumerateMe}}test{{/if}}{{/each}}")(data);
            });
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/674
        [Theory]
        [InlineData("{{#if}}yes{{/if}}", "#if")]
        [InlineData("{{#unless}}yes{{/unless}}", "#unless")]
        [InlineData("{{^if}}yes{{/if}}", "#if")]
        [InlineData("{{#if a b}}yes{{/if}}", "#if")]
        [InlineData("{{#unless a b}}yes{{/unless}}", "#unless")]
        public void TestConditionalBlockWithWrongArgumentCountException(string source, string helper)
        {
            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile(source);
            });

            Assert.Contains($"{helper} requires exactly one argument", exception.Message);
        }

        [Theory]
        [InlineData("{{#if a includeZero=true}}yes{{else}}no{{/if}}", "yes")]
        [InlineData("{{#unless a includeZero=true}}yes{{else}}no{{/unless}}", "no")]
        public void TestConditionalBlockWithHashArgumentStillCompiles(string source, string expected)
        {
            Assert.Equal(expected, Handlebars.Compile(source)(new { a = 0 }));
        }

        [Fact]
        public void TestNonClosingIgnoreBlockException()
        {
            Assert.Throws<HandlebarsParserException>(() =>
            {
                Handlebars.Compile("{{ [test }}")(new { });
            });
        }
   }
}
