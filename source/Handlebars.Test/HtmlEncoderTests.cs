using HandlebarsDotNet;
using System;
using System.Globalization;
using System.IO;
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
        [InlineData("�a", "&#65533;a")]
        [InlineData("\"", "&quot;")]
        [InlineData("&a&", "&amp;a&amp;")]
        [InlineData("a&a", "a&amp;a")]
        public void EncodeTest(string input, string expected)
        {
            // Arrange
            var htmlEncoder = new HtmlEncoder(CultureInfo.InvariantCulture);
            using var writer = new StringWriter();

            // Act
            htmlEncoder.Encode(input, writer);

            // Assert
            Assert.Equal(expected, writer.ToString());
        }
    }
}
