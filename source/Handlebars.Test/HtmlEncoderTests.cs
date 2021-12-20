using System.IO;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class HtmlEncoderTests
    {
        [Theory]
        // Escape chars based on https://github.com/handlebars-lang/handlebars.js/blob/master/lib/handlebars/utils.js
        [InlineData("&", "&amp;")]
        [InlineData("<", "&lt;")]
        [InlineData(">", "&gt;")]
        [InlineData("\"", "&quot;")]
        [InlineData("'", "&#x27;")]
        [InlineData("`", "&#x60;")]
        [InlineData("=", "&#x3D;")]

        // Don't escape.
        [InlineData("â", "â")]
        public void EscapeCorrectCharacters(string input, string expected)
        {
            var compatibility = new Compatibility { UseLegacyHandlebarsNetHtmlEncoding = false };
            var htmlEncoder = new HtmlEncoder(compatibility);
            using var writer = new StringWriter();

            htmlEncoder.Encode(input, writer);

            Assert.Equal(expected, writer.ToString());
        }

        [Theory]
        [InlineData("&", "&amp;")]
        [InlineData("<", "&lt;")]
        [InlineData(">", "&gt;")]
        [InlineData("\"", "&quot;")]
        [InlineData("â", "&#226;")]

        // Don't escape.
        [InlineData("'", "'")]
        [InlineData("`", "`")]
        [InlineData("=", "=")]
        public void EscapeCorrectCharactersHandlebarsNetLegacyRules(string input, string expected)
        {
            var compatibility = new Compatibility { UseLegacyHandlebarsNetHtmlEncoding = true };
            var htmlEncoder = new HtmlEncoder(compatibility);
            using var writer = new StringWriter();

            htmlEncoder.Encode(input, writer);

            Assert.Equal(expected, writer.ToString());
        }

        [Fact]
        public void EscapeCorrectCharacters_LateChangeConfig()
        {
            var compatibility = new Compatibility { UseLegacyHandlebarsNetHtmlEncoding = true };
            var htmlEncoder = new HtmlEncoder(compatibility);
            using var writer = new StringWriter();

            compatibility.UseLegacyHandlebarsNetHtmlEncoding = false;
            htmlEncoder.Encode("â", writer);

            Assert.Equal("â", writer.ToString());
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, "")]
        [InlineData(" ", " ")]
        [InlineData("&", "&amp;")]
        [InlineData("<", "&lt;")]
        [InlineData(">", "&gt;")]
        [InlineData("  >  ","  &gt;  ")]
        [InlineData("�", "&#65533;")]
        [InlineData("�a", "&#65533;a")]
        [InlineData("\"", "&quot;")]
        [InlineData("&a&", "&amp;a&amp;")]
        [InlineData("a&a", "a&amp;a")]
        public void EncodeTestHandlebarsNetLegacyRules(string input, string expected)
        {
            // Arrange
            var compatibility = new Compatibility { UseLegacyHandlebarsNetHtmlEncoding = true };
            var htmlEncoder = new HtmlEncoder(compatibility);
            using var writer = new StringWriter();

            // Act
            htmlEncoder.Encode(input, writer);

            // Assert
            Assert.Equal(expected, writer.ToString());
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, "")]
        [InlineData(" ", " ")]
        [InlineData("&", "&amp;")]
        [InlineData("<", "&lt;")]
        [InlineData(">", "&gt;")]
        [InlineData("  >  ", "  &gt;  ")]
        [InlineData("\"", "&quot;")]
        [InlineData("&a&", "&amp;a&amp;")]
        [InlineData("a&a", "a&amp;a")]
        public void EncodeTest(string input, string expected)
        {
            // Arrange
            var compatibility = new Compatibility { UseLegacyHandlebarsNetHtmlEncoding = false };
            var htmlEncoder = new HtmlEncoder(compatibility);
            using var writer = new StringWriter();

            // Act
            htmlEncoder.Encode(input, writer);

            // Assert
            Assert.Equal(expected, writer.ToString());
        }
    }
}
