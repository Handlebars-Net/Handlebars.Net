using Xunit;

namespace HandlebarsDotNet.Test
{
    public class PartialResolverTests
    {
        public class CustomPartialResolver : IPartialTemplateResolver
        {
            public bool TryRegisterPartial(IHandlebars env, string partialName, string templatePath)
            {
                if (partialName == "person")
                {
                    env.RegisterTemplate("person", "{{name}}");
                    return true;
                }

                return false;
            }
        }

        [Fact]
        public void BasicPartial()
        {
            string source = "Hello, {{>person}}!";

            var handlebars = Handlebars.Create(new HandlebarsConfiguration
            {
                PartialTemplateResolver = new CustomPartialResolver()
            });


            var template = handlebars.Compile(source);

            var data = new {
                name = "Marc"
            };
            
            var result = template(data);
            Assert.Equal("Hello, Marc!", result);
        }
    }
}

