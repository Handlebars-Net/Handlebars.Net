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

            for (var index = 0; index < expected.Length; index++)
            {
                Assert.True(split[index] == expected[index]);
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
        [InlineData("abc", 'a', "bc")]
        [InlineData("cccde", 'c', "de")]
        public void TrimStartSubstring(string input, char trimChar, string expected)
        {
            var substring = new Substring(input);
            substring = Substring.TrimStart(substring, trimChar);
            
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
        public void TrimEndSubstring(string input, char trimChar, string expected)
        {
            var substring = new Substring(input);
            substring = Substring.TrimEnd(substring, trimChar);
            
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
        
        [Theory]
        [InlineData("cabc", 'c', "ab")]
        [InlineData("cccccabccccc", 'c', "ab")]
        public void TrimSubstring(string input, char trimChar, string expected)
        {
            var substring = new Substring(input);
            substring = Substring.Trim(substring, trimChar);
            
            Assert.Equal(expected, substring.ToString());
        }
    }
}