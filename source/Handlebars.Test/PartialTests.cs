using System;
using System.Collections.Generic;
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
            string source = "Hello, {{>person first='Pete' last=\"Sampras\"}}!";

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
        public void BasicPartialWithStringParameterIncludingExpressionChars()
        {
            string source = "Hello, {{>person first='Pe ({~te~}) '}}!";

            var template = Handlebars.Compile(source);

            var partialSource = "{{first}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(null);
            Assert.AreEqual("Hello, Pe ({~te~}) !", result);
        }

        [Test]
        public void DynamicPartial()
        {
            string source = "Hello, {{> (partialNameHelper)}}!";

            Handlebars.RegisterHelper("partialNameHelper", (writer, context, args) =>
            {
                writer.WriteSafeString("partialName");
            });

            using (var reader = new StringReader("world"))
            {
                var partial = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("partialName", partial);
            }

            var template = Handlebars.Compile(source);
            var data = new { };
            var result = template(data);
            Assert.AreEqual("Hello, world!", result);
        }

        [Test]
        public void DynamicPartialWithHelperArguments()
        {
            string source = "Hello, {{> (concat 'par' 'tial' item1='Na' item2='me')}}!";

            Handlebars.RegisterHelper("concat", (writer, context, args) =>
            {
                var hash = args[2] as Dictionary<string, object>;
                writer.WriteSafeString(string.Concat(args[0], args[1], hash["item1"], hash["item2"]));
            });

            using (var reader = new StringReader("world"))
            {
                var partial = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("partialName", partial);
            }

            var template = Handlebars.Compile(source);
            var data = new { };
            var result = template(data);
            Assert.AreEqual("Hello, world!", result);
        }

        [Test]
        public void DynamicPartialWithContext()
        {
            var source = "Hello, {{> (lookup name) context }}!";

            Handlebars.RegisterHelper("lookup", (output, context, arguments) =>
            {
                output.WriteSafeString(arguments[0]);
            });

            var template = Handlebars.Compile(source);

            using (var reader = new StringReader("{{first}} {{last}}"))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("test", partialTemplate);
            }

            var data = new
            {
                name = "test",
                context = new
                {
                    first = "Marc",
                    last = "Smith"
                }
            };

            var result = template(data);
            Assert.AreEqual("Hello, Marc Smith!", result);
        }

        [Test]
        public void DynamicPartialWithParameters()
        {
            var source = "Hello, {{> (lookup name) first='Marc' last='Smith' }}!";

            Handlebars.RegisterHelper("lookup", (output, context, arguments) =>
            {
                output.WriteSafeString(arguments[0]);
            });

            var template = Handlebars.Compile(source);

            using (var reader = new StringReader("{{first}} {{last}}"))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("test", partialTemplate);
            }

            var data = new
            {
                name = "test"
            };

            var result = template(data);
            Assert.AreEqual("Hello, Marc Smith!", result);
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

        [Test]
        public void BasicPartialWithStringParametersAndImplicitContext()
        {
            string source = "Hello, {{>person lastName='Smith'}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                firstName = "Marc",
                lastName = "Jones"
            };

            var partialSource = "{{firstName}} {{lastName}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(data);
            Assert.AreEqual("Hello, Marc Smith!", result);
        }

        [Test]
        public void BasicPartialWithEmptyParameterDoesNotFallback()
        {
            string source = "Hello, {{>person lastName=test}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                firstName = "Marc",
                lastName = "Jones"
            };

            var partialSource = "{{firstName}} {{lastName}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(data);
            Assert.AreEqual("Hello, Marc !", result);
        }

        [Test]
        public void BasicPartialWithIncompleteChildContextDoesNotFallback()
        {
            string source = "Hello, {{>person leadDev}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                firstName = "Pete",
                lastName = "Jones",
                leadDev = new
                {
                    firstName = "Marc"
                }
            };

            var partialSource = "{{firstName}} {{lastName}}";
            using (var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person", partialTemplate);
            }

            var result = template(data);
            Assert.AreEqual("Hello, Marc !", result);
        }

        [Test]
        public void BasicBlockPartial()
        {
            string source = "Hello, {{#>person1}}friend{{/person1}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                firstName = "Pete",
                lastName = "Jones"
            };

            var result1 = template(data);
            Assert.AreEqual ("Hello, friend!", result1);

            var partialSource = "{{firstName}} {{lastName}}";
            using (var reader = new StringReader(partialSource)) {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("person1", partialTemplate);
            }

            var result2 = template(data);
            Assert.AreEqual("Hello, Pete Jones!", result2);
        }

        [Test]
        public void BasicBlockPartialWithArgument()
        {
            string source = "Hello, {{#>person2 arg='Todd'}}friend{{/person2}}!";

            var template = Handlebars.Compile (source);

            var data = new {
                firstName = "Pete",
                lastName = "Jones"
            };

            var result1 = template (data);
            Assert.AreEqual ("Hello, friend!", result1);

            var partialSource = "{{arg}}";
            using (var reader = new StringReader (partialSource)) {
                var partialTemplate = Handlebars.Compile (reader);
                Handlebars.RegisterTemplate ("person2", partialTemplate);
            }

            var result2 = template (data);
            Assert.AreEqual ("Hello, Todd!", result2);
        }
    }
}

