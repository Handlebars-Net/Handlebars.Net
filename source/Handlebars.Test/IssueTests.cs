using System.Dynamic;
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
            var handlebars = Handlebars.Create();
            var expected = "Helper";
            handlebars.RegisterHelper("foo", (context, arguments) => expected);

            var template = handlebars.Compile("{{foo}}");
            var data = new {foo = "Variable"};
            var actual = template(data);
            Assert.Equal(expected, actual);
        }

        // Issue https://github.com/rexm/Handlebars.Net/issues/350
        [Fact]
        public void LateBoundHelperWithSameNameVariable()
        {
            var handlebars = Handlebars.Create();
            var template = handlebars.Compile("{{amoeba}}");

            Assert.Equal("Variable", template(new {amoeba = "Variable"}));

            handlebars.RegisterHelper("amoeba", (writer, context, arguments) => { writer.Write("Helper"); });

            Assert.Equal("Helper", template(new {amoeba = "Variable"}));
        }
        
        // Issue https://github.com/rexm/Handlebars.Net/issues/350
        [Fact]
        public void LateBoundHelperWithSameNameVariablePath()
        {
            var handlebars = Handlebars.Create();
            var expected = "Variable";
            var template = handlebars.Compile("{{amoeba.a}}");
            var data = new {amoeba = new {a = expected}};
            
            var actual = template(data);
            Assert.Equal(expected, actual);

            handlebars.RegisterHelper("amoeba", (context, arguments) => "Helper");
            
            actual = template(data);
            Assert.Equal(expected, actual);
        }
        
        // Issue https://github.com/rexm/Handlebars.Net/issues/354
        [Fact]
        public void BlockHelperWithInversion()
        {
            string source = "{{^test input}}empty{{else}}not empty{{/test}}";

            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("test", (output, options, context, arguments) =>
            {
                if (HandlebarsUtils.IsTruthy(arguments[0]))
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse(output, context);
                }
            });

            var template = handlebars.Compile(source);
    
            Assert.Equal("empty", template(null));
            Assert.Equal("empty", template(new { otherInput = 1 }));
            Assert.Equal("not empty", template(new { input = 1 }));
        }
    }
}