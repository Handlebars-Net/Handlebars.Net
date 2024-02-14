using System.Linq;
using HandlebarsDotNet.PathStructure;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class PathInfoTests
    {
        [Theory]
        [InlineData("abc")]
        [InlineData("ab1c")]
        [InlineData("a")]
        public void SimplePath(string input)
        {
            var pathInfo = PathInfo.Parse(input);
            
            Assert.Equal(input, pathInfo.Path);
            Assert.Equal(input.Trim('[', ']'), pathInfo.TrimmedPath);
            Assert.False(pathInfo.IsVariable);
            var chainSegment = Assert.Single(pathInfo.Segments.SelectMany(o => o.PathChain));
            Assert.NotNull(chainSegment);
            Assert.Equal(input, chainSegment.ToString());
        }
        
        [Fact]
        public void DotPath()
        {
            var input = "a.b.1.c";
            
            var pathInfo = PathInfo.Parse(input);
            
            Assert.Equal(input, pathInfo.Path);

            var parts = input.Split('.');
            var pathChain = pathInfo.Segments.SelectMany(o => o.PathChain).ToArray();
            for (var index = 0; index < pathChain.Length; index++)
            {
                Assert.Equal(parts[index], pathChain[index]);
            }
        }
        
        [Theory]
        [InlineData("a", new [] {"a"})]
        [InlineData("a.c", new [] {"a.c"})]
        [InlineData("a//c", new [] {"a", "", "c"})]
        [InlineData("a/b/c", new [] {"a", "b", "c"})]
        [InlineData("a/b.c/d", new [] {"a", "b.c", "d"})]
        [InlineData("a/[b.c]/d", new [] {"a", "[b.c]", "d"})]
        [InlineData("a/[b[.]c]/d", new [] {"a", "[b[.]c]", "d"})]
        [InlineData("a/[b.c].[b/c]/d", new [] {"a", "[b.c].[b/c]", "d"})]
        [InlineData("a/[b/c]/d", new [] {"a", "[b/c]", "d"})]
        [InlineData("a/[b.c/d]/e", new [] {"a", "[b.c/d]", "e"})]
        [InlineData("a/[b.c/d/e/f]/e", new [] {"a", "[b.c/d/e/f]", "e"})]
        [InlineData("a/[a//b/c.d/e]/e", new [] {"a", "[a//b/c.d/e]", "e"})]
        [InlineData("a/[b/c/d].[e/f/g]/h", new [] {"a", "[b/c/d].[e/f/g]", "h"})]
        public void SlashPath(string input, string[] expected)
        {
            var pathInfo = PathInfo.Parse(input);
            
            Assert.Equal(input, pathInfo.Path);
            
            for (var index = 0; index < pathInfo.Segments.Length; index++)
            {
                Assert.Equal(expected[index], pathInfo.Segments[index].ToString());
            }
        }
    }
}
