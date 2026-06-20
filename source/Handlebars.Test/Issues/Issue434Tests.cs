using System.Dynamic;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class Issue434Tests
    {
        [Fact]
        public void Issue434_CaseSensitiveLookupWithSameSpellingVariables()
        {
            var h = Handlebars.Create();
            var template = h.Compile("{{TEST}} {{test}}");
            dynamic data = new ExpandoObject();
            data.TEST = "Upper";
            data.test = "Lower";
            var result = template(data);
            var parts = result.Split(' ');
            Assert.Equal("Upper", parts[0]);
            Assert.Equal("Lower", parts[1]);
        }
    }
}
