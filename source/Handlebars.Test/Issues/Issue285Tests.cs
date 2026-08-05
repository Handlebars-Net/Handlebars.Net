using Xunit;

namespace HandlebarsDotNet.Test.Issues
{
    /// <summary>
    /// Regression tests for GitHub issue #285:
    /// Support the <c>includeZero=true</c> hash argument on the built-in <c>#if</c> helper,
    /// matching Handlebars.js behaviour (https://handlebarsjs.com/guide/builtin-helpers.html#if).
    /// </summary>
    public class Issue285Tests
    {
        [Fact]
        public void IfWithIncludeZeroTrue_ZeroInt_RendersBlock()
        {
            var source = "{{#if value includeZero=true}}yes{{else}}no{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = 0 });
            Assert.Equal("yes", result);
        }

        [Fact]
        public void IfWithIncludeZeroTrue_ZeroDouble_RendersBlock()
        {
            var source = "{{#if value includeZero=true}}yes{{else}}no{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = 0.0 });
            Assert.Equal("yes", result);
        }

        [Fact]
        public void IfWithIncludeZeroTrue_NonZeroInt_RendersBlock()
        {
            var source = "{{#if value includeZero=true}}yes{{else}}no{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = 1 });
            Assert.Equal("yes", result);
        }

        [Fact]
        public void IfWithIncludeZeroFalse_ZeroInt_DoesNotRenderBlock()
        {
            var source = "{{#if value includeZero=false}}yes{{else}}no{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = 0 });
            Assert.Equal("no", result);
        }

        [Fact]
        public void IfWithoutIncludeZero_ZeroInt_StillTreatedAsFalsy()
        {
            var source = "{{#if value}}yes{{else}}no{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = 0 });
            Assert.Equal("no", result);
        }

        [Fact]
        public void IfWithIncludeZeroTrue_NullValue_StillTreatedAsFalsy()
        {
            var source = "{{#if value includeZero=true}}yes{{else}}no{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = (object?)null });
            Assert.Equal("no", result);
        }

        [Fact]
        public void IfWithIncludeZeroTrue_EmptyString_StillTreatedAsFalsy()
        {
            var source = "{{#if value includeZero=true}}yes{{else}}no{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = string.Empty });
            Assert.Equal("no", result);
        }

        [Fact]
        public void IfWithIncludeZeroTrue_FalseBool_StillTreatedAsFalsy()
        {
            var source = "{{#if value includeZero=true}}yes{{else}}no{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = false });
            Assert.Equal("no", result);
        }

        [Fact]
        public void IfWithIncludeZeroTrue_TrueBool_RendersBlock()
        {
            var source = "{{#if value includeZero=true}}yes{{else}}no{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = true });
            Assert.Equal("yes", result);
        }

        [Fact]
        public void IfWithHashArgument_DoesNotCrash()
        {
            // Regression test: passing any hash arg to #if previously threw
            // InvalidOperationException: "Sequence contains more than one element".
            var source = "{{#if value includeZero=true}}yes{{/if}}";
            var template = Handlebars.Compile(source);
            var result = template(new { value = 42 });
            Assert.Equal("yes", result);
        }
    }
}
