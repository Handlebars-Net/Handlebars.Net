using System;
using System.Collections.Generic;
using HandlebarsDotNet.Features;
using Xunit;

namespace HandlebarsDotNet.Test
{
    /// <summary>
    /// Tests derived from the Handlebars.js spec (handlebars-lang/handlebars.js/spec/)
    /// covering behaviors not exercised by the existing test suite.
    /// See HandlebarsSpec.md in this directory for the full behavioral specification.
    ///
    /// IMPLEMENTATION GAPS — tests that assert the current (non-spec-compliant) behavior
    /// are annotated with [SPEC GAP]. Each gap documents:
    ///   SPEC:    what canonical Handlebars.js specifies
    ///   CURRENT: what Handlebars.Net currently produces
    ///   SOURCE:  where in the library the divergence lives
    ///   COMPAT:  backward-compatibility risk of closing the gap
    /// </summary>
    public class HandlebarsSpecCoverageTests
    {
        // ─────────────────────────────────────────────────────────────
        // 1. HTML ENCODING EDGE CASES
        // ─────────────────────────────────────────────────────────────

        // The default encoder (HtmlEncoder) encodes all 7 characters per Handlebars.js spec:
        // &, <, >, ", ', `, and =. This was fixed in issue #546 — previously the default was
        // HtmlEncoderLegacy which omitted ', `, and =. Users who need legacy behavior can
        // configure TextEncoder = new HtmlEncoderLegacy() explicitly.

        [Fact]
        public void HtmlEncoding_SingleQuote_DefaultEncoderSpec()
        {
            // Default HtmlEncoder encodes single quotes per Handlebars.js spec (issue #546)
            var hbs = Handlebars.Create();
            Assert.Equal("it&#x27;s", hbs.Compile("{{val}}")(new { val = "it's" }));
        }

        [Fact]
        public void HtmlEncoding_SingleQuote_LegacyEncoderPassesThrough()
        {
            // Legacy encoder does NOT encode single quotes (opt-in for backward compatibility)
            var hbs = Handlebars.Create(new HandlebarsConfiguration { TextEncoder = new HtmlEncoderLegacy() });
            Assert.Equal("it's", hbs.Compile("{{val}}")(new { val = "it's" }));
        }

        [Fact]
        public void HtmlEncoding_Backtick_DefaultEncoderSpec()
        {
            var hbs = Handlebars.Create();
            Assert.Equal("a&#x60;b", hbs.Compile("{{val}}")(new { val = "a`b" }));
        }

        [Fact]
        public void HtmlEncoding_Backtick_LegacyEncoderPassesThrough()
        {
            var hbs = Handlebars.Create(new HandlebarsConfiguration { TextEncoder = new HtmlEncoderLegacy() });
            Assert.Equal("a`b", hbs.Compile("{{val}}")(new { val = "a`b" }));
        }

        [Fact]
        public void HtmlEncoding_Equals_DefaultEncoderSpec()
        {
            var hbs = Handlebars.Create();
            Assert.Equal("a&#x3D;b", hbs.Compile("{{val}}")(new { val = "a=b" }));
        }

        [Fact]
        public void HtmlEncoding_Equals_LegacyEncoderPassesThrough()
        {
            var hbs = Handlebars.Create(new HandlebarsConfiguration { TextEncoder = new HtmlEncoderLegacy() });
            Assert.Equal("a=b", hbs.Compile("{{val}}")(new { val = "a=b" }));
        }

        [Fact]
        public void HtmlEncoding_AllSpecialCharsAtOnce_DefaultEncoderSpec()
        {
            // Default HtmlEncoder encodes all 7 characters per Handlebars.js spec (issue #546)
            var hbs = Handlebars.Create();
            Assert.Equal("&amp;&lt;&gt;&quot;&#x27;&#x60;&#x3D;", hbs.Compile("{{val}}")(new { val = "&<>\"'`=" }));
        }

        [Fact]
        public void HtmlEncoding_AllSpecialCharsAtOnce_LegacyEncoder()
        {
            // Legacy encoder: &, <, >, " are encoded; ', `, = are not
            var hbs = Handlebars.Create(new HandlebarsConfiguration { TextEncoder = new HtmlEncoderLegacy() });
            Assert.Equal("&amp;&lt;&gt;&quot;'`=", hbs.Compile("{{val}}")(new { val = "&<>\"'`=" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void HtmlEncoding_TripleStashSkipsEncoding(IHandlebars hbs)
        {
            var template = hbs.Compile("{{{val}}}");
            Assert.Equal("&<>\"'`=", template(new { val = "&<>\"'`=" }));
        }

        // ─────────────────────────────────────────────────────────────
        // 2. BACKSLASH ESCAPING
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BackslashEscape_SingleBeforeMustache(IHandlebars hbs)
        {
            // \{{name}} → {{name}} (the expression is not evaluated)
            var template = hbs.Compile(@"\{{name}}");
            Assert.Equal("{{name}}", template(new { name = "Alice" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BackslashEscape_EscapedThenNonEscaped(IHandlebars hbs)
        {
            // \{{name}} {{name}} → {{name}} Alice
            var template = hbs.Compile(@"\{{name}} {{name}}");
            Assert.Equal("{{name}} Alice", template(new { name = "Alice" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BackslashEscape_DoubleBackslashBeforeMustache(IHandlebars hbs)
        {
            // \\{{name}} → \Alice  (one backslash literal, then evaluated)
            var template = hbs.Compile(@"\\{{name}}");
            Assert.Equal(@"\Alice", template(new { name = "Alice" }));
        }

        // ─────────────────────────────────────────────────────────────
        // 3. COMMENTS
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Comment_InlineProducesNoOutput(IHandlebars hbs)
        {
            var template = hbs.Compile("a{{! this is a comment }}b");
            Assert.Equal("ab", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Comment_BlockCanContainClosingBraces(IHandlebars hbs)
        {
            // Block comment {{!-- --}} can contain }} without closing
            var template = hbs.Compile("a{{!-- this has }} inside it --}}b");
            Assert.Equal("ab", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Comment_BlockCanContainMustacheExpression(IHandlebars hbs)
        {
            var template = hbs.Compile("a{{!-- {{foo}} --}}b");
            Assert.Equal("ab", template(new { foo = "ignored" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Comment_InlineDoesNotStripWhitespace(IHandlebars hbs)
        {
            // Without ~, comment does not strip surrounding whitespace
            var template = hbs.Compile("a {{! comment }} b");
            Assert.Equal("a  b", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Comment_StandaloneStripsItsLine(IHandlebars hbs)
        {
            // A comment on its own line strips the entire line
            var template = hbs.Compile("a\n{{! comment }}\nb");
            Assert.Equal("a\nb", template(new { }));
        }

        // ─────────────────────────────────────────────────────────────
        // 4. WHITESPACE CONTROL ON ELSE AND COMMENTS
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void WhitespaceControl_OnElse(IHandlebars hbs)
        {
            // {{~else~}} strips surrounding whitespace from the else transition
            var template = hbs.Compile("{{#if val~}} A {{~else~}} B {{~/if}}");
            Assert.Equal("A", template(new { val = true }));
            Assert.Equal("B", template(new { val = false }));
        }

        // [SPEC GAP] Whitespace control (tilde) is not applied around comment tokens.
        // SPEC:    {{~! comment ~}} and {{~!-- comment --~}} should strip adjacent whitespace.
        // CURRENT: Inline comments: tilde is present in the token but the whitespace stripping
        //          pipeline does not remove surrounding whitespace → "a b" instead of "ab".
        //          Block comments ({{~!-- ... --~}}): the tilde prefix causes a parse error
        //          ("Reached end of template in the middle of a comment") because the lexer
        //          looks for "{{!--" literally and does not accept the "{{~!--" variant.
        // SOURCE:  Whitespace-stripping logic does not cover comment expression node types;
        //          the comment start token is matched on "{{!--" without tilde variants.
        // COMPAT:  LOW — purely additive; no existing code depends on this NOT working.

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void WhitespaceControl_OnInlineComment(IHandlebars hbs)
        {
            // SPEC: expects "ab" — tilde strips the spaces on both sides of the comment
            // CURRENT: whitespace is not stripped around comments → "a b"
            var template = hbs.Compile("a {{~! comment ~}} b");
            Assert.Equal("a b", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void WhitespaceControl_OnBlockComment(IHandlebars hbs)
        {
            // SPEC: expects "ab" — tilde strips whitespace around block comments
            // CURRENT: {{~!-- is a parse error at compile time
            Assert.Throws<HandlebarsParserException>(() => hbs.Compile("a {{~!-- block comment --~}} b"));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void WhitespaceControl_StripsNewlines(IHandlebars hbs)
        {
            var template = hbs.Compile("1\n{{foo~}} \n\n 23\n{{bar}}4");
            Assert.Equal("1\nA23\nB4", template(new { foo = "A", bar = "B" }));
        }

        // ─────────────────────────────────────────────────────────────
        // 5. LITERAL VALUES IN #if
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_LiteralTrue(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if true}}yes{{else}}no{{/if}}");
            Assert.Equal("yes", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_LiteralFalse(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if false}}yes{{else}}no{{/if}}");
            Assert.Equal("no", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_LiteralOneIsTruthy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if 1}}yes{{else}}no{{/if}}");
            Assert.Equal("yes", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_LiteralZeroIsFalsy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if 0}}yes{{else}}no{{/if}}");
            Assert.Equal("no", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_EmptyArrayIsFalsy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if val}}yes{{else}}no{{/if}}");
            Assert.Equal("no", template(new { val = Array.Empty<string>() }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_NonEmptyArrayIsTruthy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if val}}yes{{else}}no{{/if}}");
            Assert.Equal("yes", template(new { val = new[] { "a" } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_ZeroIsFalsy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if val}}yes{{else}}no{{/if}}");
            Assert.Equal("no", template(new { val = 0 }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_NonZeroIsTruthy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if val}}yes{{else}}no{{/if}}");
            Assert.Equal("yes", template(new { val = -1 }));
            Assert.Equal("yes", template(new { val = 0.1 }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_EmptyStringIsFalsy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if val}}yes{{else}}no{{/if}}");
            Assert.Equal("no", template(new { val = "" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void If_WhitespaceStringIsTruthy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if val}}yes{{else}}no{{/if}}");
            Assert.Equal("yes", template(new { val = " " }));
        }

        // ─────────────────────────────────────────────────────────────
        // 6. #unless
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Unless_BasicFalseRendersBody(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#unless val}}no{{/unless}}");
            Assert.Equal("no", template(new { val = false }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Unless_TrueSkipsBody(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#unless val}}no{{/unless}}");
            Assert.Equal("", template(new { val = true }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Unless_WithElse(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#unless val}}falsy{{else}}truthy{{/unless}}");
            Assert.Equal("falsy", template(new { val = false }));
            Assert.Equal("truthy", template(new { val = true }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Unless_NullIsFalsy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#unless val}}missing{{else}}found{{/unless}}");
            Assert.Equal("missing", template(new { val = (string?)null }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Unless_EmptyArrayIsFalsy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#unless items}}empty{{else}}has items{{/unless}}");
            Assert.Equal("empty", template(new { items = Array.Empty<string>() }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Unless_ZeroIsFalsy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#unless val}}zero{{else}}nonzero{{/unless}}");
            Assert.Equal("zero", template(new { val = 0 }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Unless_EmptyStringIsFalsy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#unless val}}empty{{else}}nonempty{{/unless}}");
            Assert.Equal("empty", template(new { val = "" }));
        }

        // ─────────────────────────────────────────────────────────────
        // 7. #each EDGE CASES
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Each_EmptyObjectGoesToElse(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each obj}}{{@key}}{{else}}empty{{/each}}");
            Assert.Equal("empty", template(new { obj = new { } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Each_EmptyObjectWithNoElseProducesNothing(IHandlebars hbs)
        {
            var template = hbs.Compile("before{{#each obj}}{{@key}}{{/each}}after");
            Assert.Equal("beforeafter", template(new { obj = new { } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Each_ObjectKeyWithHtmlCharsIsEncoded(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each obj}}{{@key}}={{this}} {{/each}}");
            var data = new Dictionary<string, string> { { "<b>", "val" } };
            Assert.Equal("&lt;b&gt;=val ", template(new { obj = data }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Each_NullCollectionGoesToElse(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each items}}{{this}}{{else}}nothing{{/each}}");
            Assert.Equal("nothing", template(new { items = (string[]?)null }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Each_FalseCollectionGoesToElse(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each items}}{{this}}{{else}}nothing{{/each}}");
            // false is falsy so #each goes to else
            Assert.Equal("nothing", template(new { items = false }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Each_SingleElementBothFirstAndLast(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each items}}first={{@first}} last={{@last}}{{/each}}");
            // In .NET, @first and @last are booleans that render as True/False
            Assert.Equal("first=True last=True", template(new { items = new[] { "only" } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Each_BlockParamsOnObject(IHandlebars hbs)
        {
            // Dictionary<string, int> has a platform bug where key block param isn't bound on Linux/Windows; use object.
            var template = hbs.Compile("{{#each obj as |itemVal itemKey|}}{{itemKey}}={{itemVal}} {{/each}}");
            var data = new Dictionary<string, object> { { "x", 1 }, { "y", 2 } };
            Assert.Equal("x=1 y=2 ", template(new { obj = data }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Each_NestedWithParentIndex(IHandlebars hbs)
        {
            // Correct syntax: @../index (@ prefix comes first, then ../ navigation)
            // {{../@index}} is wrong — Handlebars.Net uses {{@../index}} not {{../@index}}
            var template = hbs.Compile("{{#each outer}}{{#each inner}}{{@../index}}-{{@index}} {{/each}}{{/each}}");
            var data = new
            {
                outer = new[]
                {
                    new { inner = new[] { "a", "b" } },
                    new { inner = new[] { "c" } }
                }
            };
            Assert.Equal("0-0 0-1 1-0 ", template(data));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Each_IndexCommaLastSeparatorPattern(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each items}}{{this}}{{#unless @last}},{{/unless}}{{/each}}");
            Assert.Equal("a,b,c", template(new { items = new[] { "a", "b", "c" } }));
        }

        // ─────────────────────────────────────────────────────────────
        // 8. #with EDGE CASES
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void With_NullGoesToElse(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#with val}}yes{{else}}no{{/with}}");
            Assert.Equal("no", template(new { val = (string?)null }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void With_FalseGoesToElse(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#with val}}yes{{else}}no{{/with}}");
            Assert.Equal("no", template(new { val = false }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void With_EmptyArrayGoesToElse(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#with val}}yes{{else}}no{{/with}}");
            Assert.Equal("no", template(new { val = Array.Empty<string>() }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void With_MissingPropertyGoesToElse(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#with missing}}yes{{else}}no{{/with}}");
            Assert.Equal("no", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void With_EmptyObjectIsTruthy(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#with val}}yes{{else}}no{{/with}}");
            Assert.Equal("yes", template(new { val = new { } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void With_BlockParamsAlias(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#with person as |p|}}{{p.name}}{{/with}}");
            Assert.Equal("Alice", template(new { person = new { name = "Alice" } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void With_BlockParamsAliasAccessesContextProperties(IHandlebars hbs)
        {
            // Inside #with, unaliased properties resolve to the with-context (person)
            var template = hbs.Compile("{{#with person as |p|}}{{p.name}} is {{age}}{{/with}}");
            Assert.Equal("Erik is 42", template(new { person = new { name = "Erik", age = 42 } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void With_ParentNavigation(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#with address}}{{street}}, {{../city}}{{/with}}");
            Assert.Equal("123 Main, Springfield", template(new
            {
                city = "Springfield",
                address = new { street = "123 Main" }
            }));
        }

        // ─────────────────────────────────────────────────────────────
        // 9. BLOCK HELPERS — INVERSE, HASH, PRIORITY
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BlockHelper_WithInverse(IHandlebars hbs)
        {
            hbs.RegisterHelper("ifCond", (writer, options, context, args) =>
            {
                if (args[0] is bool b && b)
                    options.Template(writer, context);
                else
                    options.Inverse(writer, context);
            });
            var template = hbs.Compile("{{#ifCond flag}}yes{{else}}no{{/ifCond}}");
            Assert.Equal("yes", template(new { flag = true }));
            Assert.Equal("no", template(new { flag = false }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BlockHelper_HashArguments(IHandlebars hbs)
        {
            hbs.RegisterHelper("tag", (writer, options, context, args) =>
            {
                // Hash arguments come as a Dictionary<string,object> in args[0] when there are no positional args
                var hash = args[0] as Dictionary<string, object> ?? new Dictionary<string, object>();
                var cls = hash.TryGetValue("class", out var c) ? c?.ToString() ?? "" : "";
                var id  = hash.TryGetValue("id", out var i) ? i?.ToString() ?? "" : "";
                writer.WriteSafeString($"<div class=\"{cls}\" id=\"{id}\">");
                options.Template(writer, context);
                writer.WriteSafeString("</div>");
            });
            var template = hbs.Compile("{{#tag class=\"active\" id=\"main\"}}content{{/tag}}");
            Assert.Equal("<div class=\"active\" id=\"main\">content</div>", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Helper_ReturnValue(IHandlebars hbs)
        {
            hbs.RegisterHelper("echo", (context, args) => args[0]);
            var template = hbs.Compile("{{echo greeting}}");
            Assert.Equal("Hello", template(new { greeting = "Hello" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Helper_SafeStringBypassesEncoding(IHandlebars hbs)
        {
            hbs.RegisterHelper("bold", (writer, context, args) =>
            {
                writer.WriteSafeString($"<b>{args[0]}</b>");
            });
            var template = hbs.Compile("{{bold name}}");
            Assert.Equal("<b>Alice</b>", template(new { name = "Alice" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Helper_WriterOutputIsHtmlEncoded(IHandlebars hbs)
        {
            hbs.RegisterHelper("unsafe", (writer, context, args) =>
            {
                writer.Write("<b>bold</b>");
            });
            var template = hbs.Compile("{{unsafe}}");
            Assert.Equal("&lt;b&gt;bold&lt;/b&gt;", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Helper_InverseIsAlwaysSafeToCallEvenWithoutElse(IHandlebars hbs)
        {
            hbs.RegisterHelper("myBlock", (writer, options, context, args) =>
            {
                // options.Inverse should be callable even without {{else}} in the template
                options.Inverse(writer, context);
            });
            var template = hbs.Compile("{{#myBlock}}body{{/myBlock}}");
            // Since we call Inverse and there's no else block, output should be empty
            Assert.Equal("", template(new { }));
        }

        // ─────────────────────────────────────────────────────────────
        // 10. MISSING HELPER HOOK
        // ─────────────────────────────────────────────────────────────

        [Fact]
        public void MissingHelperHook_InterceptsMissingHelper()
        {
            var hbs = Handlebars.Create(new HandlebarsConfiguration()
                .RegisterMissingHelperHook(
                    helperMissing: (in HelperOptions options, in Context context, in Arguments args) =>
                        $"[missing:{options.Name}]"
                )
            );

            var template = hbs.Compile("{{unknownHelper world}}");
            var result = template(new { world = "world" });
            Assert.Equal("[missing:unknownHelper]", result);
        }

        // ─────────────────────────────────────────────────────────────
        // 11. SUBEXPRESSIONS
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Subexpression_ResultPassedToOuterHelper(IHandlebars hbs)
        {
            hbs.RegisterHelper("upper", (context, args) => args[0]?.ToString()?.ToUpper());
            hbs.RegisterHelper("wrap", (context, args) => $"[{args[0]}]");
            var template = hbs.Compile("{{wrap (upper name)}}");
            Assert.Equal("[ALICE]", template(new { name = "Alice" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Subexpression_Nested(IHandlebars hbs)
        {
            hbs.RegisterHelper("trim", (context, args) => args[0]?.ToString()?.Trim());
            hbs.RegisterHelper("upper", (context, args) => args[0]?.ToString()?.ToUpper());
            hbs.RegisterHelper("wrap", (context, args) => $"[{args[0]}]");
            var template = hbs.Compile("{{wrap (upper (trim val))}}");
            Assert.Equal("[HELLO]", template(new { val = "  hello  " }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Subexpression_AsHashValue(IHandlebars hbs)
        {
            hbs.RegisterHelper("join", (context, args) => string.Join("-", args[0], args[1]));
            hbs.RegisterHelper("upper", (context, args) => args[0]?.ToString()?.ToUpper());
            hbs.RegisterHelper("tag", (writer, context, args) =>
            {
                writer.WriteSafeString($"<span class=\"{args[0]}\">");
            });

            var template = hbs.Compile("{{tag (join prefix suffix)}}");
            Assert.Equal("<span class=\"a-b\">", template(new { prefix = "a", suffix = "b" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Subexpression_UsedInIf(IHandlebars hbs)
        {
            hbs.RegisterHelper("isReady", (context, args) => args[0]?.ToString() == "ready");
            var template = hbs.Compile("{{#if (isReady status)}}yes{{else}}no{{/if}}");
            Assert.Equal("yes", template(new { status = "ready" }));
            Assert.Equal("no", template(new { status = "pending" }));
        }

        // ─────────────────────────────────────────────────────────────
        // 12. PATHS — EDGE CASES
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Path_HyphenatedIdentifier(IHandlebars hbs)
        {
            var template = hbs.Compile("{{foo-bar}}");
            // foo-bar is a valid identifier
            var data = new Dictionary<string, object> { { "foo-bar", "baz" } };
            Assert.Equal("baz", template(data));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Path_NullIntermediateRendersEmpty(IHandlebars hbs)
        {
            var template = hbs.Compile("{{a.b.c}}");
            Assert.Equal("", template(new { a = (object?)null }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Path_DeepNullIntermediateRendersEmpty(IHandlebars hbs)
        {
            var template = hbs.Compile("{{person.name}}");
            Assert.Equal("", template(new { person = (object?)null }));
        }

        // [SPEC GAP] Boolean false renders as "False" (capital F) instead of "false" (lowercase).
        // SPEC:    Handlebars.js renders {{val}} with val=false as the string "false".
        // CURRENT: Renders as "False" — .NET bool.ToString() returns "False" not "false".
        //          Note: val=false IS still falsy for {{#if val}}, which is correct.
        // SOURCE:  The value formatting pipeline uses the default .NET ToString() on bool values
        //          without special-casing booleans to match JS casing conventions.
        // COMPAT:  MEDIUM — any code that renders boolean values via {{expr}} and checks the
        //          output string for "false" (lowercase) would break. Similarly @first and @last
        //          data variables currently render as "True"/"False".

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Path_FalseRendersAsFalseString(IHandlebars hbs)
        {
            // SPEC: expects "false" (lowercase); CURRENT: .NET bool.ToString() gives "False"
            var template = hbs.Compile("{{val}}");
            Assert.Equal("False", template(new { val = false }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Path_ZeroRendersAsZeroString(IHandlebars hbs)
        {
            var template = hbs.Compile("{{val}}");
            Assert.Equal("0", template(new { val = 0 }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Path_NullRendersAsEmpty(IHandlebars hbs)
        {
            var template = hbs.Compile("{{val}}");
            Assert.Equal("", template(new { val = (string?)null }));
        }

        // ─────────────────────────────────────────────────────────────
        // 13. DATA VARIABLES — @root DEEP NESTING
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DataVar_RootInDoubleNestedEach(IHandlebars hbs)
        {
            // Correct syntax: @../index — the @ prefix must come before ../ path navigation
            var template = hbs.Compile(
                "{{#each outer}}{{#each inner}}{{@root.title}}/{{@../index}}/{{@index}} {{/each}}{{/each}}"
            );
            var data = new
            {
                title = "T",
                outer = new[]
                {
                    new { inner = new[] { 1, 2 } },
                    new { inner = new[] { 3 } }
                }
            };
            Assert.Equal("T/0/0 T/0/1 T/1/0 ", template(data));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DataVar_RootAccessibleFromWithContext(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#with person}}{{name}} / {{@root.title}}{{/with}}");
            Assert.Equal("Alice / Boss", template(new { title = "Boss", person = new { name = "Alice" } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DataVar_CustomDataPassedAtRenderTime(IHandlebars hbs)
        {
            hbs.RegisterHelper("getData", (in EncodedTextWriter writer, in HelperOptions options, in Context context, in Arguments args) =>
            {
                var val = options.Data.Value<string>("custom");
                writer.Write(val ?? "");
            });
            var template = hbs.Compile("{{getData}}");
            var result = template(new { }, new { custom = "hello" });
            Assert.Equal("hello", result);
        }

        // ─────────────────────────────────────────────────────────────
        // 14. INTERACTIONS
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Interaction_IfWithFirstInEach(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each list}}{{#if @first}}FIRST:{{/if}}{{this}} {{/each}}");
            Assert.Equal("FIRST:a b c ", template(new { list = new[] { "a", "b", "c" } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Interaction_EachWithLookup(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each people}}{{.}} → {{lookup ../cities @index}} {{/each}}");
            Assert.Equal("Alice → NYC Bob → LA ", template(new
            {
                people = new[] { "Alice", "Bob" },
                cities = new[] { "NYC", "LA" }
            }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Interaction_WithSubexpressionLookup(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#with (lookup cities key)~}}{{name}} ({{country}}){{/with}}");
            Assert.Equal("Darmstadt (Germany)", template(new
            {
                key = "darmstadt",
                cities = new Dictionary<string, object>
                {
                    { "darmstadt", new { name = "Darmstadt", country = "Germany" } }
                }
            }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Interaction_PartialInsideEachWithAtIndex(IHandlebars hbs)
        {
            hbs.RegisterTemplate("person", "{{name}}({{@index}})");
            var template = hbs.Compile("{{#each people}}{{> person}} {{/each}}");
            var result = template(new { people = new[] { new { name = "Alice" }, new { name = "Bob" } } });
            Assert.Equal("Alice(0) Bob(1) ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Interaction_IfElseIfChain(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#if a}}A{{else if b}}B{{else}}C{{/if}}");
            Assert.Equal("A", template(new { a = true, b = false }));
            Assert.Equal("B", template(new { a = false, b = true }));
            Assert.Equal("C", template(new { a = false, b = false }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Interaction_InlinePartialWithEach(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#*inline \"row\"}}{{name}}{{/inline}}{{#each people}}{{> row}} {{/each}}");
            var result = template(new { people = new[] { new { name = "Alice" }, new { name = "Bob" } } });
            Assert.Equal("Alice Bob ", result);
        }

        // ─────────────────────────────────────────────────────────────
        // 15. PARTIALS — BLOCK PARTIAL WITH @partial-block
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Partial_BlockFallbackWhenNotRegistered(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#> missingPartial}}default content{{/missingPartial}}");
            Assert.Equal("default content", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Partial_BlockWithAtPartialBlock(IHandlebars hbs)
        {
            hbs.RegisterTemplate("wrapper", "<div>{{> @partial-block}}</div>");
            var template = hbs.Compile("{{#> wrapper}}inner{{/wrapper}}");
            Assert.Equal("<div>inner</div>", template(new { }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Partial_ExplicitContext(IHandlebars hbs)
        {
            hbs.RegisterTemplate("person", "{{name}}");
            var template = hbs.Compile("{{> person data}}");
            Assert.Equal("Bob", template(new { data = new { name = "Bob" }, name = "Root" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Partial_HashParams(IHandlebars hbs)
        {
            hbs.RegisterTemplate("greet", "Hello, {{name}}!");
            var template = hbs.Compile("{{> greet name=\"World\"}}");
            Assert.Equal("Hello, World!", template(new { name = "Alice" }));
        }

        // ─────────────────────────────────────────────────────────────
        // 16. INVERTED SECTIONS
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void InvertedSection_FalseValue(IHandlebars hbs)
        {
            var template = hbs.Compile("{{^val}}inverted{{/val}}");
            Assert.Equal("inverted", template(new { val = false }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void InvertedSection_EmptyArray(IHandlebars hbs)
        {
            var template = hbs.Compile("{{^items}}none{{/items}}");
            Assert.Equal("none", template(new { items = Array.Empty<string>() }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void InvertedSection_TruthyValueProducesNoOutput(IHandlebars hbs)
        {
            var template = hbs.Compile("{{^val}}inverted{{/val}}");
            Assert.Equal("", template(new { val = true }));
        }

        // [SPEC GAP] {{^}} (standalone caret with no name) inside a block body is not an else separator.
        // SPEC:    {{#val}}truthy{{^}}falsy{{/val}} is equivalent to {{#val}}truthy{{else}}falsy{{/val}};
        //          when val is truthy only "truthy" renders; when falsy only "falsy" renders.
        // CURRENT: {{^}} is resolved as a path expression for the current context rather than as
        //          an else marker. For val=true this produces "yes" + context.ToString() + "no"
        //          because the caret resolves to the anonymous object `new { val = true }` and
        //          no inverse block is registered, so "no" is literal text in the body.
        //          For val=false the entire body is skipped (falsy) and the inverse is empty → "".
        //          WORKAROUND: use {{#val}}truthy{{else}}falsy{{/val}} — {{else}} works correctly.
        // SOURCE:  The block section accumulator does not recognize empty {{^}} as an inverse
        //          separator for non-each blocks; the caret falls through to path resolution.
        // COMPAT:  LOW — {{else}} is the canonical and documented form; {{^}} with no name is
        //          a rarely-used edge case and changing its behavior would be a narrow breakage.

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void InvertedSection_InlineCaretSyntax(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#val}}yes{{^}}no{{/val}}");
            // SPEC: val=true → "yes", val=false → "no"
            // CURRENT: {{^}} resolves to context.ToString() (html-encoded per active encoder),
            //          then "no" appears as plain text after it — neither branch is separated.
            //          val=true:  output is "yes" + encoded(context.ToString()) + "no"
            //          val=false: output is "" — falsy skips the body; no inverse block is registered
            var resultTrue = template(new { val = true });
            Assert.StartsWith("yes", resultTrue);   // truthy branch text IS output...
            Assert.EndsWith("no", resultTrue);      // ...but so is the "false" branch text
            Assert.NotEqual("yes", resultTrue);     // it would be just "yes" if {{^}} worked as else
            Assert.Equal("", template(new { val = false }));
        }

        // ─────────────────────────────────────────────────────────────
        // 17. SEGMENT LITERALS / SPECIAL KEY NAMES
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void SegmentLiteral_KeyWithSpaces(IHandlebars hbs)
        {
            var template = hbs.Compile("{{[foo bar]}}");
            var data = new Dictionary<string, object> { { "foo bar", "value" } };
            Assert.Equal("value", template(data));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void SegmentLiteral_NestedWithSpaces(IHandlebars hbs)
        {
            var template = hbs.Compile("{{obj.[a b]}}");
            var data = new
            {
                obj = new Dictionary<string, object> { { "a b", "found" } }
            };
            Assert.Equal("found", template(data));
        }

        // ─────────────────────────────────────────────────────────────
        // 18. LOOKUP HELPER
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Lookup_UndefinedKeyRendersEmpty(IHandlebars hbs)
        {
            var template = hbs.Compile("{{lookup obj key}}");
            Assert.Equal("", template(new
            {
                obj = new Dictionary<string, string> { { "a", "val" } },
                key = "missing"
            }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Lookup_DynamicArrayIndex(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each people}}{{lookup ../cities @index}} {{/each}}");
            Assert.Equal("NYC LA ", template(new
            {
                people = new[] { "Alice", "Bob" },
                cities = new[] { "NYC", "LA" }
            }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Lookup_OnUndefinedObjectRendersEmpty(IHandlebars hbs)
        {
            var template = hbs.Compile("{{lookup missing key}}");
            Assert.Equal("", template(new { key = "a" }));
        }

        // ─────────────────────────────────────────────────────────────
        // 19. LOG HELPER
        // ─────────────────────────────────────────────────────────────

        // [SPEC GAP] The {{log}} built-in helper is not implemented.
        // SPEC:    {{log expr}} should pass expr to the platform logger and produce no template output.
        // CURRENT: Throws HandlebarsRuntimeException: "Template references a helper that cannot be
        //          resolved. Helper 'log'" — log is not registered as a built-in helper.
        // SOURCE:  BuildInHelpersFeature.cs — the log helper is absent from the registration list.
        // COMPAT:  LOW — adding log as a registered no-op (or logger-delegating) helper is purely
        //          additive; currently {{log}} templates cannot be compiled without throwing, so
        //          no existing code can be relying on the current exception-throwing behavior.

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Log_ProducesNoOutput(IHandlebars hbs)
        {
            // SPEC: should compile and render "beforeafter"
            // CURRENT: compiles successfully but throws HandlebarsRuntimeException at render time
            var template = hbs.Compile("before{{log message}}after");
            Assert.Throws<HandlebarsRuntimeException>(() => template(new { message = "hello" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Log_WithStringLiteralProducesNoOutput(IHandlebars hbs)
        {
            // SPEC: should compile and render "beforeafter"
            // CURRENT: compiles successfully but throws HandlebarsRuntimeException at render time
            var template = hbs.Compile("before{{log 'debug message'}}after");
            Assert.Throws<HandlebarsRuntimeException>(() => template(new { }));
        }

        // ─────────────────────────────────────────────────────────────
        // 20. BLOCK PARAMS — SCOPE AND SHADOWING
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BlockParams_ShadowsContextPropertyWithSameName(IHandlebars hbs)
        {
            var template = hbs.Compile("{{#each list as |name|}}{{name}} {{/each}}{{name}}");
            Assert.Equal("inner1 inner2 outer", template(new
            {
                name = "outer",
                list = new[] { "inner1", "inner2" }
            }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BlockParams_NestedScopesShadowIndependently(IHandlebars hbs)
        {
            var template = hbs.Compile(
                "{{#each outer as |item|}}[{{#each item.inner as |item|}}{{item}} {{/each}}]{{/each}}"
            );
            var data = new
            {
                outer = new[]
                {
                    new { inner = new[] { "a", "b" } },
                    new { inner = new[] { "c" } }
                }
            };
            Assert.Equal("[a b ][c ]", template(data));
        }

        // ─────────────────────────────────────────────────────────────
        // 21. COMPLETE TRUTHINESS REFERENCE
        //     (mirrors the edge case table in the spec)
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Truthiness_NullIsFalsy(IHandlebars hbs)
        {
            var t = hbs.Compile("{{#if v}}T{{else}}F{{/if}}");
            Assert.Equal("F", t(new { v = (object?)null }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Truthiness_ZeroIsFalsy(IHandlebars hbs)
        {
            var t = hbs.Compile("{{#if v}}T{{else}}F{{/if}}");
            Assert.Equal("F", t(new { v = 0 }));
            Assert.Equal("F", t(new { v = 0.0 }));
            Assert.Equal("F", t(new { v = 0m }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Truthiness_EmptyStringIsFalsy(IHandlebars hbs)
        {
            var t = hbs.Compile("{{#if v}}T{{else}}F{{/if}}");
            Assert.Equal("F", t(new { v = "" }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Truthiness_EmptyArrayIsFalsy(IHandlebars hbs)
        {
            var t = hbs.Compile("{{#if v}}T{{else}}F{{/if}}");
            Assert.Equal("F", t(new { v = Array.Empty<string>() }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Truthiness_EmptyObjectIsTruthy(IHandlebars hbs)
        {
            var t = hbs.Compile("{{#if v}}T{{else}}F{{/if}}");
            Assert.Equal("T", t(new { v = new { } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Truthiness_NonEmptyListIsTruthy(IHandlebars hbs)
        {
            var t = hbs.Compile("{{#if v}}T{{else}}F{{/if}}");
            Assert.Equal("T", t(new { v = new[] { 1 } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Truthiness_TrueIsTruthy(IHandlebars hbs)
        {
            var t = hbs.Compile("{{#if v}}T{{else}}F{{/if}}");
            Assert.Equal("T", t(new { v = true }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void Truthiness_NonzeroNumberIsTruthy(IHandlebars hbs)
        {
            var t = hbs.Compile("{{#if v}}T{{else}}F{{/if}}");
            Assert.Equal("T", t(new { v = 1 }));
            Assert.Equal("T", t(new { v = -1 }));
            Assert.Equal("T", t(new { v = 0.5 }));
        }

        // ─────────────────────────────────────────────────────────────
        // 22. RENDER-VALUE vs FALSY DISTINCTION
        //     (false renders as "false" but is falsy for #if)
        // ─────────────────────────────────────────────────────────────

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void RenderVsFalsy_FalseRendersButIsFalsyForIf(IHandlebars hbs)
        {
            // SPEC: {{val}} with val=false → "false" (lowercase); CURRENT: "False" (.NET casing)
            Assert.Equal("False", hbs.Compile("{{val}}")(new { val = false }));
            Assert.Equal("no", hbs.Compile("{{#if val}}yes{{else}}no{{/if}}")(new { val = false }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void RenderVsFalsy_ZeroRendersButIsFalsyForIf(IHandlebars hbs)
        {
            Assert.Equal("0", hbs.Compile("{{val}}")(new { val = 0 }));
            Assert.Equal("no", hbs.Compile("{{#if val}}yes{{else}}no{{/if}}")(new { val = 0 }));
        }
    }
}
