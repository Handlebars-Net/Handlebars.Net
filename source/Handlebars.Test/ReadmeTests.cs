using System.Collections.Generic;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class ReadmeTests
    {
        [Fact]
        public void RegisterBlockHelper()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("StringEqualityBlockHelper", (output, options, context, arguments) => 
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{#StringEqualityBlockHelper}} helper must have exactly two arguments");
                }

                var left = arguments.At<string>(0);
                var right = arguments[1] as string;
                if (left == right) options.Template(output, context);
                else options.Inverse(output, context);
            });

            var animals = new Dictionary<string, string> 
            {
                {"Fluffy", "cat" },
                {"Fido", "dog" },
                {"Chewy", "hamster" }
            };

            var template = "{{#each this}}The animal, {{@key}}, {{#StringEqualityBlockHelper @value 'dog'}}is a dog{{else}}is not a dog{{/StringEqualityBlockHelper}}.\n{{/each}}";
            var compiledTemplate = handlebars.Compile(template);
            string templateOutput = compiledTemplate(animals);
            
            Assert.Equal(
                "The animal, Fluffy, is not a dog.\n" + 
                         "The animal, Fido, is a dog.\n" + 
                         "The animal, Chewy, is not a dog.\n", 
                templateOutput
            );
        }
        
        [Fact]
        public void ElseChainingWithBlockHelper()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("StringEqualityBlockHelper", (output, options, context, arguments) =>
            {
                if (arguments.Length != 2)
                {
                    throw new HandlebarsException("{{#StringEqualityBlockHelper}} helper must have exactly two arguments");
                }

                var left = arguments.At<string>(0);
                var right = arguments[1] as string;
                if (left == right) options.Template(output, context);
                else options.Inverse(output, context);
            });

            var template = "{{#StringEqualityBlockHelper value 'dog'}}is a dog{{else StringEqualityBlockHelper value 'cat'}}is a cat{{else}}is something else{{/StringEqualityBlockHelper}}";
            var compiledTemplate = handlebars.Compile(template);

            Assert.Equal("is a dog", compiledTemplate(new { value = "dog" }));
            Assert.Equal("is a cat", compiledTemplate(new { value = "cat" }));
            Assert.Equal("is something else", compiledTemplate(new { value = "hamster" }));
        }

        [Fact]
        public void ElseIfChaining()
        {
            var handlebars = Handlebars.Create();
            var template = handlebars.Compile(
                "{{#if isDog}}is a dog{{else if isCat}}is a cat{{else}}is something else{{/if}}");

            Assert.Equal("is a dog", template(new { isDog = true, isCat = false }));
            Assert.Equal("is a cat", template(new { isDog = false, isCat = true }));
            Assert.Equal("is something else", template(new { isDog = false, isCat = false }));
        }

        [Fact]
        public void RegisterHelper()
        {
            var source = @"Click here: {{link_to}}";
            
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("link_to", (writer, context, parameters) =>
            {
                writer.WriteSafeString($"<a href='{context["url"]}'>{context["text"]}</a>");
            });
            
            var template = handlebars.Compile(source);

            var data = new {
                url = "https://github.com/rexm/handlebars.net",
                text = "Handlebars.Net"
            };

            var result = template(data);
            
            Assert.Equal($"Click here: <a href='{data.url}'>{data.text}</a>", result);
        }
    }
}