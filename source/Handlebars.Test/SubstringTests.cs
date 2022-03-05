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
                Assert.Equal(expected[index++], split.Current) ;
            }
        }

        [Theory]
        [InlineData("ab//bc", '/', new []{ "ab", "bc" })]
        [InlineData("/a//c/d/e//", '/', new []{ "a", "c", "d", "e" })]
        public void SplitRemoveEmpty(string input, char splitChar, string[] expected)
        {
            var substring = new Substring(input);
            var split = Substring.Split(substring, splitChar, System.StringSplitOptions.RemoveEmptyEntries);

            var index = 0;
            while (split.MoveNext())
            {
                Assert.Equal(expected[index++], split.Current);
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
        
        [Theory]
        [InlineData("abc", 'c', true)]
        [InlineData("abccccc", 'd', false)]
        public void Contains(string input, char trimChar, bool expected)
        {
            var actual = Substring.Contains(input, trimChar);
            
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [InlineData("abc", 'c', false)]
        [InlineData("abccccc", 'b', true)]
        public void StartsWith(string input, char c, bool expected)
        {
            var substring = new Substring(input, 1, 2);
            var actual = Substring.StartsWith(substring, c);
            
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [InlineData("abc", 'c', true)]
        [InlineData("abcd", 'd', false)]
        public void EndsWith(string input, char c, bool expected)
        {
            var substring = new Substring(input, 1, 2);
            var actual = Substring.EndsWith(substring, c);
            
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [InlineData("abc", 'c', 2)]
        [InlineData("abcc", 'c', 2)]
        [InlineData("abccccc", 'd', -1)]
        public void IndexOf(string input, char trimChar, int expected)
        {
            var actual = Substring.IndexOf(input, trimChar);
            
            Assert.Equal(expected, actual);
        }
        
        [Theory]
        [InlineData("abc", 'c', 2)]
        [InlineData("abcc", 'c', 3)]
        [InlineData("abcc", 'a', 0)]
        [InlineData("abccccc", 'd', -1)]
        public void LastIndexOf(string input, char trimChar, int expected)
        {
            var actual = Substring.LastIndexOf(input, trimChar);
            
            Assert.Equal(expected, actual);
        }
    }
}