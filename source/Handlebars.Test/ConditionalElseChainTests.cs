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

        // A bare "{{else name}}" follows handlebars.js: it is "{{else}}{{#name}}...{{/name}}".
        // https://github.com/Handlebars-Net/Handlebars.Net/issues/695
        [Theory]
        [InlineData("{{#if a}}A{{else b}}B{{/if}}", true, false, true, "A")]
        [InlineData("{{#if a}}A{{else b}}B{{/if}}", false, false, true, "B")]
        [InlineData("{{#if a}}A{{else b}}B{{/if}}", false, false, false, "")]
        [InlineData("{{#unless a}}A{{else b}}B{{/unless}}", false, false, true, "A")]
        [InlineData("{{#unless a}}A{{else b}}B{{/unless}}", true, false, true, "B")]
        [InlineData("{{#unless a}}A{{else b}}B{{/unless}}", true, false, false, "")]
        [InlineData("{{#if a}}A{{else b}}B{{else}}C{{/if}}", false, false, false, "C")]
        [InlineData("{{#if a}}A{{else b}}B{{/if}}", false, true, false, "B")]
        [InlineData("{{#unless a}}A{{else b}}B{{/unless}}", true, true, false, "B")]
        public void BareElseNameIsResolvedAsAValue(string source, bool a, bool legacy, bool b, string expected)
        {
            var handlebars = CreateWithHelpers();
            handlebars.Configuration.Compatibility.LegacyBareElseNameBehavior = legacy;
            var template = handlebars.Compile(source);

            Assert.Equal(expected, template(new { a, b }));
        }

        [Theory]
        [InlineData("{{#if a}}A{{else items}}[{{this}}]{{/if}}", false, "[1][2]")]
        [InlineData("{{#unless a}}A{{else items}}[{{this}}]{{/unless}}", false, "[1][2]")]
        [InlineData("{{#if a}}A{{else items}}[{{x}}]{{/if}}", true, "[outer]")]
        [InlineData("{{#unless a}}A{{else items}}[{{x}}]{{/unless}}", true, "[outer]")]
        public void BareElseNameIteratesAList(string source, bool legacy, string expected)
        {
            var handlebars = CreateWithHelpers();
            handlebars.Configuration.Compatibility.LegacyBareElseNameBehavior = legacy;
            var template = handlebars.Compile(source);

            var a = source.StartsWith("{{#unless");
            Assert.Equal(expected, template(new { a, items = new[] { 1, 2 }, x = "outer" }));
        }

        [Theory]
        [InlineData(false, "B")]
        [InlineData(true, "B")]
        public void BareElseNameInvokesARegisteredBlockHelper(bool legacy, string expected)
        {
            var handlebars = Handlebars.Create();
            handlebars.Configuration.Compatibility.LegacyBareElseNameBehavior = legacy;
            var invoked = false;
            handlebars.RegisterHelper("track", (output, options, context, arguments) =>
            {
                invoked = true;
                options.Template(output, context);
            });
            var template = handlebars.Compile("{{#if a}}A{{else track}}B{{/if}}");

            Assert.Equal(expected, template(new { a = false }));
            Assert.Equal(!legacy, invoked);
        }

        [Theory]
        [InlineData("{{#if a}}A{{else if}}B{{/if}}", "if")]
        [InlineData("{{#if a}}A{{else unless}}B{{/if}}", "unless")]
        [InlineData("{{#unless a}}A{{else if}}B{{/unless}}", "if")]
        [InlineData("{{#unless a}}A{{else unless}}B{{/unless}}", "unless")]
        public void ConditionlessElseIfIsACompilerError(string source, string keyword)
        {
            var handlebars = CreateWithHelpers();

            var exception = Assert.Throws<HandlebarsCompilerException>(() => handlebars.Compile(source));
            Assert.Contains($"#{keyword} requires exactly one argument", exception.Message);
            Assert.Contains($"'{{{{else {keyword}}}}}'", exception.Message);
            Assert.Contains("Configuration.Compatibility.LegacyBareElseNameBehavior = true", exception.Message);
        }

        // With the opt-out, a bare name after "else" (including a condition-less "{{else if}}")
        // renders as a plain "{{else}}", as in earlier Handlebars.Net versions.
        [Theory]
        [InlineData("{{#if a}}A{{else ifNot}}B{{/if}}", true, false)]
        [InlineData("{{#if a}}A{{else if}}B{{/if}}", true, false)]
        [InlineData("{{#if a}}A{{else unless}}B{{/if}}", true, false)]
        [InlineData("{{#unless a}}A{{else if}}B{{/unless}}", false, true)]
        [InlineData("{{#unless a}}A{{else unless}}B{{/unless}}", false, true)]
        public void LegacyBareElseNameRendersAsPlainElse(string source, bool showsA, bool showsB)
        {
            var handlebars = CreateWithHelpers();
            handlebars.Configuration.Compatibility.LegacyBareElseNameBehavior = true;
            var template = handlebars.Compile(source);

            Assert.Equal("A", template(new { a = showsA }));
            Assert.Equal("B", template(new { a = showsB }));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ElseAfterPlainElseIsACompilerErrorForBareNames(bool legacy)
        {
            var handlebars = CreateWithHelpers();
            handlebars.Configuration.Compatibility.LegacyBareElseNameBehavior = legacy;

            Assert.Throws<HandlebarsCompilerException>(() =>
                handlebars.Compile("{{#if a}}A{{else}}B{{else b}}C{{/if}}"));
        }

        // Realistic case from https://github.com/Handlebars-Net/Handlebars.Net/issues/695: the "if" was left out.
        [Theory]
        [InlineData(false, false, false, "")]
        [InlineData(false, false, true, "Moderator tools")]
        [InlineData(true, false, false, "Moderator tools")]
        [InlineData(false, true, false, "Admin tools")]
        public void MissingIfAfterElseOnlyRendersForTruthyName(bool legacy, bool isAdmin, bool isModerator, string expected)
        {
            var handlebars = Handlebars.Create();
            handlebars.Configuration.Compatibility.LegacyBareElseNameBehavior = legacy;
            var template = handlebars.Compile("{{#if user.isAdmin}}Admin tools{{else user.isModerator}}Moderator tools{{/if}}");

            Assert.Equal(expected, template(new { user = new { isAdmin, isModerator } }));
        }

        // Under non-conditional blocks a bare "{{else name}}" desugars in both modes.
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void BareElseNameUnderEachIsUnaffectedByLegacyFlag(bool legacy)
        {
            var handlebars = Handlebars.Create();
            handlebars.Configuration.Compatibility.LegacyBareElseNameBehavior = legacy;
            var template = handlebars.Compile("{{#each list}}x{{else b}}B{{/each}}");

            Assert.Equal("B", template(new { list = new int[0], b = true }));
            Assert.Equal("", template(new { list = new int[0], b = false }));
        }

        // A non-path first argument keeps the earlier "else if" on the remaining arguments in both modes.
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void NonPathElseNameIsUnaffectedByLegacyFlag(bool legacy)
        {
            var handlebars = Handlebars.Create();
            handlebars.Configuration.Compatibility.LegacyBareElseNameBehavior = legacy;
            var template = handlebars.Compile("{{#if a}}A{{else \"x\" b}}B{{else}}C{{/if}}");

            Assert.Equal("B", template(new { a = false, b = true }));
            Assert.Equal("C", template(new { a = false, b = false }));
        }

        // Case contributed in https://github.com/Handlebars-Net/Handlebars.Net/pull/684
        [Fact]
        public void IfChainsToBuiltInWithHelper()
        {
            var template = CreateWithHelpers().Compile("{{#if a}}A{{else with b}}{{c}}{{/if}}");

            Assert.Equal("A", template(new { a = true, b = new { c = "X" }, c = "outer" }));
            Assert.Equal("X", template(new { a = false, b = new { c = "X" }, c = "outer" }));
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
