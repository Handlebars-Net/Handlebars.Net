using System;
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

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Theory]
        [InlineData("{{#if !File}} {{/if}}", "!File", "0:6")]
        [InlineData("{{#unless !File}}a{{/unless}}", "!File", "0:10")]
        [InlineData("{{#each !items}}a{{/each}}", "!items", "0:8")]
        [InlineData("{{#with !item}}a{{/with}}", "!item", "0:8")]
        [InlineData("{{#if a}}a{{else if !b}}b{{/if}}", "!b", "0:20")]
        [InlineData("{{value !other}}", "!other", "0:8")]
        [InlineData("{{value key=!other}}", "!other", "0:12")]
        [InlineData("{{> partial !other}}", "!other", "0:12")]
        public void TestNegationLikeArgumentReportsTokenAndLocation(string source, string token, string location)
        {
            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile(source);
            });

            Assert.Contains($"Unexpected '{token}' inside an expression", exception.Message);
            Assert.Contains("'#unless'", exception.Message);
            Assert.EndsWith($"Occured at: {location}", exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Theory]
        [InlineData("a {{! comment }} b", "a  b")]
        [InlineData("a {{ ! comment }} b", "a  b")]
        [InlineData("a {{!-- {{value}} --}} b", "a  b")]
        public void TestCommentsStillCompile(string source, string expected)
        {
            Assert.Equal(expected, Handlebars.Compile(source)(new { value = "v" }));
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Fact]
        public void TestNonClosingBlockExpressionReportsOpeningTagLocation()
        {
            var source = "line one" + Environment.NewLine + "  {{#each items}}test";

            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile(source);
            });

            Assert.Contains("Reached end of template before block expression '#each' was closed", exception.Message);
            Assert.EndsWith("Occured at: 1:4", exception.Message);
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Theory]
        [InlineData("{{#if 0}}test{{/unless}}", "/unless", "0:15")]
        [InlineData("{{#if 0}}test{{/if}}{{/unless}}", "/unless", "0:22")]
        [InlineData("text {{/each}}", "/each", "0:7")]
        public void TestUnmatchedClosingBlockExpressionReportsLocation(string source, string closingElement, string location)
        {
            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile(source);
            });

            Assert.Contains($"A closing element '{closingElement}' was found without a matching open element", exception.Message);
            Assert.EndsWith($"Occured at: {location}", exception.Message);
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Theory]
        [InlineData("{{File}}}", "{{", "}}}")]
        [InlineData("{{{File}}", "{{{", "}}")]
        public void TestMismatchedHandlebarsReportsBraces(string source, string opening, string closing)
        {
            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile(source);
            });

            Assert.Contains("Starting and ending handlebars do not match", exception.Message);
            Assert.Contains($"opened with '{opening}' was closed with '{closing}'", exception.Message);
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
