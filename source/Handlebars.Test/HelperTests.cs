using NUnit.Framework;
using System;
using Handlebars;

namespace Handlebars.Test
{
    [TestFixture]
    public class HelperTests
    {
        [Test]
        public void HelperWithLiteralArguments()
        {
            Handlebars.RegisterHelper("myHelper", (writer, context, args) => {
                var count = 0;
                foreach(var arg in args)
                {
                    writer.Write("\nThing {0}: {1}", ++count, arg);
                }
            });

            var source = "Here are some things: {{myHelper 'foo' 'bar'}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Here are some things: \nThing 1: foo\nThing 2: bar";

            Assert.AreEqual(expected, output);
        }

        [Test]
        public void HelperWithLiteralArgumentsWithQuotes()
        {
            var helperName = "helper-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(helperName, (writer, context, args) => {
                var count = 0;
                foreach(var arg in args)
                {
                    writer.WriteSafeString(
                        string.Format("\nThing {0}: {1}", ++count, arg));
                }
            });

            var source = "Here are some things: {{" + helperName + " 'My \"favorite\" movie' 'bar'}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Here are some things: \nThing 1: My \"favorite\" movie\nThing 2: bar";

            Assert.AreEqual(expected, output);
        }

        [Test]
        public void InversionNoKey()
        {
            var source = "{{^key}}No key!{{/key}}";
            var template = Handlebars.Compile(source);
            var output = template(new { });
            var expected = "No key!";
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void InversionFalsy()
        {
            var source = "{{^key}}Falsy value!{{/key}}";
            var template = Handlebars.Compile(source);
            var data = new
            {
                key = false
            };
            var output = template(data);
            var expected = "Falsy value!";
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void InversionEmptySequence()
        {
            var source = "{{^key}}Empty sequence!{{/key}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    key = new string[] { }
                };
            var output = template(data);
            var expected = "Empty sequence!";
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void InversionNonEmptySequence()
        {
            var source = "{{^key}}Empty sequence!{{/key}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    key = new string[] { "element" }
                };
            var output = template(data);
            var expected = "";
            Assert.AreEqual(expected, output);
        }
    }
}

