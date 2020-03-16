using System.IO;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class WhitespaceTests
    {
        [Fact]
        public void PreceedingWhitespace()
        {
            var source = "Hello, {{~name}} !";
            var template = Handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello,Handlebars.Net !", result);
        }

        [Fact]
        public void TrailingWhitespace()
        {
            var source = "Hello, {{name~}} !";
            var template = Handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Fact]
        public void PrecedingAndTrailingWhitespace()
        {
            var source = "Hello, {{~name~}} !";
            var template = Handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello,Handlebars.Net!", result);
        }

        [Fact]
        public void ComplexTest()
        {
            var source =
@"{{#each nav ~}}
  <a href=""{{url}}"">
    {{~#if test}}
      {{~title}}
    {{~else~}}
      Empty
    {{~/if~}}
  </a>
{{~/each}}";
            var h = Handlebars.Create();
            var template = h.Compile(source);
            var data = new {
                nav = new [] {
                    new {
                        url = "https://google.com",
                        test = true,
                        title = "Google"
                    },
                    new {
                        url = "https://bing.com",
                        test = false,
                        title = "Bing"
                    }
                }
            };
            var result = template(data);
            Assert.Equal(@"<a href=""https://google.com"">Google</a><a href=""https://bing.com"">Empty</a>", result);
        }

        [Fact]
        public void StandaloneEach()
        {
            var source = "Links:\n {{#each nav}}\n  <a href=\"{{url}}\">\n    {{#if test}}\n    {{title}}\n    {{else}}\n    Empty\n    {{/if}}\n  </a>\n  {{/each}}";
            var h = Handlebars.Create();
            var template = h.Compile(source);
            var data = new
            {
                nav = new[]
                {
                    new
                    {
                        url = "https://google.com",
                        test = true,
                        title = "Google"
                    },
                    new
                    {
                        url = "https://bing.com",
                        test = false,
                        title = "Bing"
                    }
                }
            };
            var result = template(data);
            Assert.Equal( "Links:\n  <a href=\"https://google.com\">\n    Google\n  </a>\n  <a href=\"https://bing.com\">\n    Empty\n  </a>\n", result);
        }

        [Fact]
        public void StandaloneSection()
        {
            var source = "  {{#none}}\n{{this}}\n{{else}}\n{{none}}\n{{/none}}  ";

            var template = Handlebars.Compile(source);

            var data = new {none = "No people"};
            var result = template(data);

            Assert.Equal("No people\n", result);
        }

        [Fact]
        public void StandaloneInvertedSection()
        {
            var source = "  {{^some}}\n{{none}}\n{{else}}\n{{none}}\n{{/some}}  ";

            var template = Handlebars.Compile(source);

            var data = new {none = "No people"};
            var result = template(data);

            Assert.Equal("No people\n", result);
        }

        [Fact]
        public void StandaloneElseSection()
        {
            var source = "{{#people}}\n{{name}}\n{{else}}\n{{none}}\n{{/people}}\n";
            var template = Handlebars.Compile(source);

            var data = new {none = "No people"};
            var result = template(data);

            Assert.Equal("No people\n", result);
        }

        [Fact]
        public void StandaloneChainedElseSection()
        {
            var source = "{{#if people}}\n{{people.name}}\n{{else if none}}\n{{none}}\n{{/if}}\n";
            var template = Handlebars.Compile(source);

            var data = new {none = "No people"};
            var result = template(data);

            Assert.Equal("No people\n", result);
        }

        [Fact]
        public void StandaloneNesting()
        {
            var source = "{{#data}}\n{{#if 'true'}}\n{{this}}\n{{/if}}\n{{/data}}\nOK.";
            var template = Handlebars.Compile(source);

            var data = new {data = new[] {1, 3, 5}};
            var result = template(data);

            Assert.Equal("1\n3\n5\nOK.", result);
        }

		[Fact]
        public void StandaloneComment()
        {
            var source = "{{#none}}\nPeople: \n{{! this is comment }}\n{{this}}\n{{/none}}\n";

            var template = Handlebars.Compile(source);

            var data = new {none = "No people"};
            var result = template(data);

            Assert.Equal("People: \nNo people\n", result);
        }

		[Fact]
        public void StandaloneConsequentComments()
        {
            var source = "{{#none}}\nPeople: \n  {{! this is comment #1 }}  \n{{! this is comment #2 }}\n {{this}}\n{{/none}}\n";

            var template = Handlebars.Compile(source);

            var data = new {none = "No people"};
            var result = template(data);

            Assert.Equal("People: \n No people\n", result);
        }

        [Fact]
        public void StandalonePartials()
        {
            string source = "Here are:\n  {{>person}} \n {{>person}}  ";

            var template = Handlebars.Compile(source);

            var data = new {name = "Marc"};
            var partialSource = "{{name}}";

            using(var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(data);
            Assert.Equal("Here are:\nMarcMarc", result);
        }

    }
}