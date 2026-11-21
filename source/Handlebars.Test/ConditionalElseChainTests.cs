using Xunit;

namespace HandlebarsDotNet.Test
{
    // Chaining "{{else name args}}" off an {{#if}} / {{#unless}} block, as handlebars.js does:
    // "{{#if a}}A{{else name args}}B{{/if}}" is equivalent to
    // "{{#if a}}A{{else}}{{#name args}}B{{/name}}{{/if}}".
    // https://github.com/Handlebars-Net/Handlebars.Net/issues/683
    public class ConditionalElseChainTests
    {
        private static IHandlebars CreateWithHelpers()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("ifEq", (output, options, context, arguments) =>
            {
                if (Equals(arguments[0]?.ToString(), arguments[1]?.ToString())) options.Template(output, context);
                else options.Inverse(output, context);
            });
            handlebars.RegisterHelper("ifNot", (output, options, context, arguments) =>
            {
                if (!HandlebarsUtils.IsTruthy(arguments[0])) options.Template(output, context);
                else options.Inverse(output, context);
            });
            return handlebars;
        }

        [Theory]
        [InlineData(true, "x", "A")]
        [InlineData(false, "x", "B")]
        [InlineData(false, "y", "C")]
        public void IfChainsToCustomBlockHelperWithElse(bool a, string b, string expected)
        {
            var template = CreateWithHelpers().Compile("{{#if a}}A{{else ifEq b \"x\"}}B{{else}}C{{/if}}");

            Assert.Equal(expected, template(new { a, b }));
        }

        [Theory]
        [InlineData(true, "y", "A")]
        [InlineData(false, "y", "B")]
        [InlineData(false, "x", "")]
        public void IfChainsToCustomBlockHelperWithoutElse(bool a, string b, string expected)
        {
            var template = CreateWithHelpers().Compile("{{#if a}}A{{else ifEq b \"y\"}}B{{/if}}");

            Assert.Equal(expected, template(new { a, b }));
        }

        [Theory]
        [InlineData(false, "x", "A")]
        [InlineData(true, "x", "B")]
        [InlineData(true, "y", "C")]
        public void UnlessChainsToCustomBlockHelper(bool a, string b, string expected)
        {
            var template = CreateWithHelpers().Compile("{{#unless a}}A{{else ifEq b \"x\"}}B{{else}}C{{/unless}}");

            Assert.Equal(expected, template(new { a, b }));
        }

        [Theory]
        [InlineData(false, false, "B")]
        [InlineData(false, true, "C")]
        public void IfChainsToSingleArgumentCustomBlockHelper(bool a, bool b, string expected)
        {
            var template = CreateWithHelpers().Compile("{{#if a}}A{{else ifNot b}}B{{else}}C{{/if}}");

            Assert.Equal(expected, template(new { a, b }));
        }

        [Theory]
        [InlineData(true, false, "A")]
        [InlineData(false, false, "B")]
        [InlineData(false, true, "C")]
        public void IfChainsToElseUnless(bool a, bool b, string expected)
        {
            var template = CreateWithHelpers().Compile("{{#if a}}A{{else unless b}}B{{else}}C{{/if}}");

            Assert.Equal(expected, template(new { a, b }));
        }

        [Theory]
        [InlineData(true, "z", false, "A")]
        [InlineData(false, "x", false, "B")]
        [InlineData(false, "z", true, "C")]
        [InlineData(false, "z", false, "D")]
        public void IfMixesElseIfAndCustomBlockHelperInOneChain(bool a, string b, bool c, string expected)
        {
            var template = CreateWithHelpers().Compile("{{#if a}}A{{else ifEq b \"x\"}}B{{else if c}}C{{else}}D{{/if}}");

            Assert.Equal(expected, template(new { a, b, c }));
        }

        [Theory]
        [InlineData(false, "z", true, "B")]
        [InlineData(false, "x", false, "C")]
        [InlineData(false, "z", false, "D")]
        public void ElseIfFollowedByCustomBlockHelperInOneChain(bool a, string b, bool c, string expected)
        {
            var template = CreateWithHelpers().Compile("{{#if a}}A{{else if c}}B{{else ifEq b \"x\"}}C{{else}}D{{/if}}");

            Assert.Equal(expected, template(new { a, b, c }));
        }

        [Fact]
        public void ElseIfWithHashParameterStillChainsInternally()
        {
            var template = CreateWithHelpers().Compile("{{#if a}}A{{else if b includeZero=true}}B{{else}}C{{/if}}");

            Assert.Equal("B", template(new { a = false, b = 0 }));
            Assert.Equal("C", template(new { a = false, b = (object)null }));
        }

        [Fact]
        public void ElseIfWithTooManyArgumentsIsACompilerError()
        {
            var handlebars = CreateWithHelpers();

            Assert.Throws<HandlebarsCompilerException>(() =>
                handlebars.Compile("{{#if a}}A{{else if b c}}B{{else}}C{{/if}}"));
        }

        [Fact]
        public void ElseAfterPlainElseIsACompilerError()
        {
            var handlebars = CreateWithHelpers();

            Assert.Throws<HandlebarsCompilerException>(() =>
                handlebars.Compile("{{#if a}}A{{else}}B{{else}}C{{/if}}"));
        }

        // A bare name after "else" (including a condition-less "{{else if}}") has always rendered as
        // a plain "{{else}}"; that is kept so existing templates keep compiling.
        [Theory]
        [InlineData("{{#if a}}A{{else ifNot}}B{{/if}}")]
        [InlineData("{{#if a}}A{{else if}}B{{/if}}")]
        [InlineData("{{#if a}}A{{else unless}}B{{/if}}")]
        public void ElseWithOnlyANameStillRendersAsPlainElse(string source)
        {
            var template = CreateWithHelpers().Compile(source);

            Assert.Equal("A", template(new { a = true }));
            Assert.Equal("B", template(new { a = false }));
        }

        [Theory]
        [InlineData(false, false, "A")]
        [InlineData(true, false, "B")]
        [InlineData(true, true, "C")]
        public void UnlessChainsToElseUnless(bool a, bool b, string expected)
        {
            var template = CreateWithHelpers().Compile("{{#unless a}}A{{else unless b}}B{{else}}C{{/unless}}");

            Assert.Equal(expected, template(new { a, b }));
        }

        [Theory]
        [InlineData(false, "x", "B")]
        [InlineData(false, "y", "C")]
        public void ChainedCustomBlockHelperUnderIfSupportsWhitespaceControl(bool a, string b, string expected)
        {
            var template = CreateWithHelpers().Compile("{{#if a~}} A {{~else ifEq b \"x\"~}} B {{~else~}} C {{~/if}}");

            Assert.Equal(expected, template(new { a, b }));
        }
    }
}
