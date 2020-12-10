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
        [InlineData("a")]
        [InlineData("a.c")]
        [InlineData("a//c")]
        [InlineData("a/b/c")]
        [InlineData("a/b.c/d")]
        [InlineData("a/[b.c]/d")]
        [InlineData("a/[b/c]/d")]
        [InlineData("a/[b.c/d]/e")]
        public void SlashPath(string input)
        {
            var pathInfo = PathInfo.Parse(input);
            
            Assert.Equal(input, pathInfo.Path);

            var parts = input.Split('/');
            for (var index = 0; index < pathInfo.Segments.Length; index++)
            {
                Assert.Equal(parts[index], pathInfo.Segments[index].ToString());
            }
        }
    }
}