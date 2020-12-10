using HandlebarsDotNet.StringUtils;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class SubstringTests
    {
        [Theory]
        [InlineData("abc", "bc")]
        [InlineData("acde", "cd")]
        public void Simple(string input, string expected)
        {
            var substring = new Substring(input, 1, 2);
            
            Assert.Equal(expected, substring.ToString());
        }
        
        [Theory]
        [InlineData("abc", "bc")]
        [InlineData("acde", "cd")]
        public void SimpleSubstring(string input, string expected)
        {
            var substring = new Substring(input);
            substring = new Substring(substring, 1, 2);
            
            Assert.Equal(expected, substring.ToString());
        }
        
        [Theory]
        [InlineData("ab/bc", '/', new []{ "ab", "bc" })]
        [InlineData("a/c/d/e", '/', new []{ "a", "c", "d", "e" })]
        public void Split(string input, char splitChar, string[] expected)
        {
            var substring = new Substring(input);
            var split = Substring.Split(substring, splitChar);

            var index = 0;
            while (split.MoveNext())
            {
                Assert.Equal(split.Current, expected[index++]);
            }
        }
        
        [Theory]
        [InlineData("abc", 'a', "bc")]
        [InlineData("cccde", 'c', "de")]
        public void TrimStart(string input, char trimChar, string expected)
        {
            var substring = Substring.TrimStart(input, trimChar);
            
            Assert.Equal(expected, substring.ToString());
        }

        [Theory]
        [InlineData("abc", 'c', "ab")]
        [InlineData("abccccc", 'c', "ab")]
        public void TrimEnd(string input, char trimChar, string expected)
        {
            var substring = Substring.TrimEnd(input, trimChar);
            
            Assert.Equal(expected, substring.ToString());
        }

        [Theory]
        [InlineData("abc", 'c', "ab")]
        [InlineData("abccccc", 'c', "ab")]
        public void Trim(string input, char trimChar, string expected)
        {
            var substring = Substring.Trim(input, trimChar);
            
            Assert.Equal(expected, substring.ToString());
        }
    }
}