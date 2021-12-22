using System.IO;
using System.Text;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class HtmlEncoderLegacyTests
    {
        private readonly HtmlEncoderLegacy _htmlEncoderLegacy;

        public HtmlEncoderLegacyTests()
        {
            _htmlEncoderLegacy = new HtmlEncoderLegacy();
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
            using var writer = new StringWriter();

            _htmlEncoderLegacy.Encode(input, writer);

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
        [InlineData("�", "&#65533;")]
        [InlineData("�a", "&#65533;a")]
        [InlineData("\"", "&quot;")]
        [InlineData("&a&", "&amp;a&amp;")]
        [InlineData("a&a", "a&amp;a")]
        public void EncodeTestHandlebarsNetLegacyRules(string input, string expected)
        {
            // Arrange
            using var writer = new StringWriter();

            // Act
            _htmlEncoderLegacy.Encode(input, writer);

            // Assert
            Assert.Equal(expected, writer.ToString());
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("a", "a")]
        [InlineData("<", "&lt;")]
        public void EncodeStringBuilderOverload(string input, string expected)
        {
            using var writer = new StringWriter();

            var inputStringBuilder = input == null ? null : new StringBuilder(input);

            _htmlEncoderLegacy.Encode(inputStringBuilder, writer);

            Assert.Equal(expected, writer.ToString());
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("a", "a")]
        [InlineData("<", "&lt;")]
        public void EncodeCharEnumeratorOverload(string input, string expected)
        {
            using var writer = new StringWriter();

            _htmlEncoderLegacy.Encode(input?.GetEnumerator(), writer);

            Assert.Equal(expected, writer.ToString());
        }
    }
}
