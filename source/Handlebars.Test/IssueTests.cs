using Xunit;

namespace HandlebarsDotNet.Test
{
    public class IssueTests
    {
        // Issue https://github.com/zjklee/Handlebars.CSharp/issues/7
        [Fact]
        public void ValueVariableShouldNotBeAccessibleFromContext()
        {
            var handlebars = Handlebars.Create();
            var render = handlebars.Compile("{{value}}");
            var output = render(new
            {
                anotherValue = "Test"
            });
            
            Assert.Equal("", output);
        }
    }
}