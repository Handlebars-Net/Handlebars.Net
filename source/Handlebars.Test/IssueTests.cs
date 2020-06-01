using System;
using System.Dynamic;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.Helpers;
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

        // Issue https://github.com/rexm/Handlebars.Net/issues/351
        [Fact]
        public void PerhapsNull()
        {
            var handlebars = Handlebars.Create();
            var render = handlebars.Compile("{{#if PerhapsNull}}It's not null{{else}}It's null{{/if}}");
            dynamic data = new ExpandoObject();
            data.PerhapsNull = null;

            var actual = render(data);
            Assert.Equal("It's null", actual);
        }

        // Issue https://github.com/rexm/Handlebars.Net/issues/350
        // the helper has priority
        // https://handlebarsjs.com/guide/expressions.html#disambiguating-helpers-calls-and-property-lookup
        [Fact]
        public void HelperWithSameNameVariable()
        {
            Handlebars.RegisterHelper("foo", (writer, context, arguments) => { writer.Write("Helper"); });

            var template = Handlebars.Compile("{{foo}}");
            var result = template(new {foo = "Variable"});
            Assert.Equal("Helper", result);
        }

        // Issue https://github.com/rexm/Handlebars.Net/issues/350
        [Fact]
        public void LateBoundHelperWithSameNameVariable()
        {
            var template = Handlebars.Compile("{{amoeba}}");

            Assert.Equal("Variable", template(new {amoeba = "Variable"}));

            Handlebars.RegisterHelper("amoeba", (writer, context, arguments) => { writer.Write("Helper"); });

            Assert.Equal("Helper", template(new {amoeba = "Variable"}));
        }
    }
}