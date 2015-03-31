using System;
using NUnit.Framework;
using System.IO;

namespace HandlebarsDotNet.Test
{
    [TestFixture]
    public class PartialTests
    {
        [Test]
        public void BasicPartial()
        {
            string source = "Hello, {{>person}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                name = "Marc"
            };

            var partialSource = "{{name}}";
            using(var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(data);
            Assert.AreEqual("Hello, Marc!", result);
        }

        [Test]
        public void BasicPartialWithContext()
        {
            string source = "Hello, {{>person leadDev}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                leadDev = new {
                    name = "Marc"
                }
            };

            var partialSource = "{{name}}";
            using(var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(data);
            Assert.AreEqual("Hello, Marc!", result);
        }


        [Test]
        public void SuperfluousWhitespace()
        {
            string source = "Hello, {{  >  person  }}!";

            var template = Handlebars.Compile(source);

            var data = new {
                name = "Marc"
            };

            var partialSource = "{{name}}";
            using(var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(data);
            Assert.AreEqual("Hello, Marc!", result);
        }
    }
}

