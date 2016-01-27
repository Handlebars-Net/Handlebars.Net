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
        public void BasicPartialWithStringParameter()
        {
            string source = "Hello, {{>person first='Pete'}}!";

            var template = Handlebars.Compile(source);

            var partialSource = "{{first}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(null);
            Assert.AreEqual("Hello, Pete!", result);
        }

        [Test]
        public void BasicPartialWithMultipleStringParameters()
        {
            string source = "Hello, {{>person first='Pete' last='Sampras'}}!";

            var template = Handlebars.Compile(source);

            var partialSource = "{{first}} {{last}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(null);
            Assert.AreEqual("Hello, Pete Sampras!", result);
        }

        [Test]
        public void BasicPartialWithContextParameter()
        {
            string source = "Hello, {{>person first=leadDev.marc}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                leadDev = new
                {
                    marc = new
                    {
                        name = "Marc"
                    }
                }
            };

            var partialSource = "{{first.name}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(data);
            Assert.AreEqual("Hello, Marc!", result);
        }

        [Test]
        public void BasicPartialWithContextAndStringParameters()
        {
            string source = "Hello, {{>person first=leadDev.marc last='Smith'}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                leadDev = new
                {
                    marc = new
                    {
                        name = "Marc"
                    }
                }
            };

            var partialSource = "{{first.name}} {{last}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(data);
            Assert.AreEqual("Hello, Marc Smith!", result);
        }

        [Test]
        public void BasicPartialWithTypedParameters()
        {
            string source = "Hello, {{>person first=1 last=true}}!";

            var template = Handlebars.Compile(source);

            var partialSource = "{{first}} {{last}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(null);
            Assert.AreEqual("Hello, 1 True!", result);
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

