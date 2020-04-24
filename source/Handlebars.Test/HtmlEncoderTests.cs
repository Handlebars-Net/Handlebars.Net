using HandlebarsDotNet;
using Xunit;

namespace Handlebars.Test
{
    public class HtmlEncoderTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData(null, "")]
        [InlineData(" ", " ")]
        [InlineData("&", "&amp;")]
        [InlineData("<", "&lt;")]
        [InlineData(">", "&gt;")]
        [InlineData("  >  ","  &gt;  ")]
        [InlineData("�", "&#65533;")]
        [InlineData("\"", "&quot;")]
        [InlineData("&a&", "&amp;a&amp;")]
        [InlineData("a&a", "a&amp;a")]
        public void EncodeTest(string input, string expected)
        {
            // Arrange
            var htmlEncoder = new HtmlEncoder();

            // Act
            var result = htmlEncoder.Encode(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
