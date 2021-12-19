using System.Text;
using HandlebarsDotNet.Extensions;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class StringBuilderExtensionsTests
    {
        [Theory]
        [InlineData(' ', "  123 ", "123 ")]
        [InlineData('c', "  1c23 ", "  1c23 ")]
        [InlineData('c', "  c123 ", "  c123 ")]
        public void TrimStart(char @char, string input, string expected)
        {
            var builder = new StringBuilder(input);

            builder.TrimStart(@char);
            
            Assert.Equal(expected, builder.ToString());
        }
        
        [Theory]
        [InlineData(' ', "  123  ", "  123")]
        [InlineData('c', "  1c23 ", "  1c23 ")]
        [InlineData('c', "  c123c ", "  c123c ")]
        public void TrimEnd(char @char, string input, string expected)
        {
            var builder = new StringBuilder(input);

            builder.TrimEnd(@char);
            
            Assert.Equal(expected, builder.ToString());
        }
    }
}