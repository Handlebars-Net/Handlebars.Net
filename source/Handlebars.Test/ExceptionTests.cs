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
            AssertLocation(exception, location);
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
            AssertLocation(exception, "1:4");
            Assert.EndsWith("  {{#each items}}test\n    ^", exception.Message);
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
            AssertLocation(exception, location);
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Fact]
        public void TestErrorMessageShowsSourceLineWithCaret()
        {
            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("<p>{{#if !File}}yes{{/if}}</p>");
            });

            Assert.Equal(0, exception.Line);
            Assert.Equal(9, exception.Column);
            Assert.EndsWith(
                "Occured at: 0:9\n\n" +
                "<p>{{#if !File}}yes{{/if}}</p>\n" +
                "         ^",
                exception.Message);
            Assert.Contains("<p>{{#if !File}}yes{{/if}}</p>", exception.ToString());
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Fact]
        public void TestErrorMessageShowsOnlyTheLineContainingTheError()
        {
            var nl = Environment.NewLine;
            var source = "<ul>" + nl + "  {{#each items}}" + nl + "    <li>{{name}}</li>" + nl + "  {{/if}}" + nl + "</ul>";

            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile(source);
            });

            Assert.Equal(3, exception.Line);
            Assert.Equal(4, exception.Column);
            Assert.EndsWith("\n\n  {{/if}}\n    ^", exception.Message);
            Assert.DoesNotContain("<li>", exception.Message);
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Theory]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void TestErrorMessageSourceLineIgnoresLineEndingStyle(string nl)
        {
            var source = "first" + nl + "{{#if !x}}{{/if}}" + nl + "last";

            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile(source);
            });

            Assert.EndsWith("\n\n{{#if !x}}{{/if}}\n      ^", exception.Message);
            Assert.DoesNotContain("first", exception.Message);
            Assert.DoesNotContain("last", exception.Message);
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Fact]
        public void TestErrorMessageCaretKeepsTabsAligned()
        {
            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("\t\t{{#if !x}}{{/if}}");
            });

            Assert.EndsWith("\n\n\t\t{{#if !x}}{{/if}}\n\t\t      ^", exception.Message);
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Fact]
        public void TestErrorMessageTruncatesLongSourceLines()
        {
            var padding = new string('a', 500);
            var source = padding + "{{#if !x}}{{/if}}" + padding;

            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile(source);
            });

            var hintLines = exception.Message.Substring(exception.Message.IndexOf("\n\n", exception.Message.IndexOf("Occured at", StringComparison.Ordinal), StringComparison.Ordinal) + 2).Split('\n');
            Assert.Equal(2, hintLines.Length);
            Assert.StartsWith("...", hintLines[0]);
            Assert.EndsWith("...", hintLines[0]);
            Assert.True(hintLines[0].Length < 200);
            var caret = hintLines[1].Length - 1;
            Assert.Equal('^', hintLines[1][caret]);
            Assert.Equal("!x}}", hintLines[0].Substring(caret, 4));
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/314
        [Fact]
        public void TestParserErrorShowsSourceLine()
        {
            var exception = Assert.Throws<HandlebarsParserException>(() =>
            {
                Handlebars.Compile("text {{!-- unterminated comment");
            });

            Assert.NotNull(exception.Line);
            Assert.NotNull(exception.Column);
            Assert.Contains("\n\ntext {{!-- unterminated comment\n", exception.Message);
            Assert.EndsWith("^", exception.Message);
        }

        [Fact]
        public void TestErrorWithoutLocationHasNoSourceLine()
        {
            var exception = Assert.Throws<HandlebarsCompilerException>(() =>
            {
                Handlebars.Compile("before {{> }} after");
            });

            Assert.Null(exception.Line);
            Assert.Null(exception.Column);
            Assert.Equal("A partial must have a name", exception.Message);
        }

        [Fact]
        public void TestExceptionsCreatedByCallersHaveNoLocation()
        {
            var exception = new HandlebarsCompilerException("custom");

            Assert.Null(exception.Line);
            Assert.Null(exception.Column);
            Assert.Equal("custom", exception.Message);
        }

        private static void AssertLocation(HandlebarsException exception, string location)
        {
            var parts = location.Split(':');
            Assert.Equal(int.Parse(parts[0]), exception.Line);
            Assert.Equal(int.Parse(parts[1]), exception.Column);
            Assert.Contains($"Occured at: {location}\n\n", exception.Message);
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
