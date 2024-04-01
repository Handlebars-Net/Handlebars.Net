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
