using Xunit;

namespace HandlebarsDotNet.Test
{
    public class Issue459Tests
    {
        [Fact]
        public void Issue459_ConditionalInPartialSeesPassedContext()
        {
            var h = Handlebars.Create();
            h.RegisterTemplate("LinkToCompany",
                "{{#if ClientCode}}IT EXISTS{{else}}IT IS NOT HERE{{/if}}");
            var template = h.Compile("{{> LinkToCompany Entity}}");
            var data = new { Entity = new { ClientCode = "TEST" } };
            Assert.Equal("IT EXISTS", template(data));
        }
    }
}
