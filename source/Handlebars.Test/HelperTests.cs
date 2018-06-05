using Xunit;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HandlebarsDotNet.Test
{
    public class HelperTests
    {
        [Fact]
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

            Assert.Equal(expected, output);
        }

        [Fact]
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

            Assert.Equal(expected, output);
        }

        [Fact]
        public void InversionNoKey()
        {
            var source = "{{^key}}No key!{{/key}}";
            var template = Handlebars.Compile(source);
            var output = template(new { });
            var expected = "No key!";
            Assert.Equal(expected, output);
        }

        [Fact]
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
            Assert.Equal(expected, output);
        }

        [Fact]
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
            Assert.Equal(expected, output);
        }

        [Fact]
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
            Assert.Equal(expected, output);
        }

        [Fact]
        public void BlockHelperWithArbitraryInversion()
        {
            var source = "{{#ifCond arg1 arg2}}Args are same{{else}}Args are not same{{/ifCond}}";

            Handlebars.RegisterHelper("ifCond", (writer, options, context, arguments) => {
                if(arguments[0] == arguments[1])
                {
                    options.Template(writer, (object)context);
                }
                else
                {
                    options.Inverse(writer, (object)context);
                }
            });

            var dataWithSameValues = new
                {
                    arg1 = "a",
                    arg2 = "a"
                };
            var dataWithDifferentValues = new
                {
                    arg1 = "a",
                    arg2 = "b"
                };

            var template = Handlebars.Compile(source);

            var outputIsSame = template(dataWithSameValues);
            var expectedIsSame = "Args are same";
            var outputIsDifferent = template(dataWithDifferentValues);
            var expectedIsDifferent = "Args are not same";

            Assert.Equal(expectedIsSame, outputIsSame);
            Assert.Equal(expectedIsDifferent, outputIsDifferent);
        }

        [Fact]
        public void HelperWithNumericArguments()
        {
            Handlebars.RegisterHelper("myHelper", (writer, context, args) => {
                var count = 0;
                foreach(var arg in args)
                {
                    writer.Write("\nThing {0}: {1}", ++count, arg);
                }
            });

            var source = "Here are some things: {{myHelper 123 4567 -98.76}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Here are some things: \nThing 1: 123\nThing 2: 4567\nThing 3: -98.76";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void HelperWithHashArgument()
        {
            Handlebars.RegisterHelper("myHelper", (writer, context, args) => {
                var hash = args[2] as Dictionary<string, object>;
                foreach(var item in hash)
                {
                    writer.Write(" {0}: {1}", item.Key, item.Value);
                }
            });

            var source = "Here are some things:{{myHelper 'foo' 'bar' item1='val1' item2='val2'}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Here are some things: item1: val1 item2: val2";

            Assert.Equal(expected, output);
        }
            
        [Fact]
        public void BlockHelperWithSubExpression()
        {
            Handlebars.RegisterHelper("isEqual", (writer, context, args) =>
            {
                writer.WriteSafeString(args[0].ToString() == args[1].ToString() ? "true" : null);
            });
        
            var source = "{{#if (isEqual arg1 arg2)}}True{{/if}}";
        
            var template = Handlebars.Compile(source);
        
            var expectedIsTrue = "True";
            var outputIsTrue = template(new { arg1 = 1, arg2 = 1 });
            Assert.Equal(expectedIsTrue, outputIsTrue);
        
            var expectedIsFalse = "";
            var outputIsFalse = template(new { arg1 = 1, arg2 = 2 });
            Assert.Equal(expectedIsFalse, outputIsFalse);
        }

        [Fact]
        public void HelperWithSegmentLiteralArguments()
        {
            Handlebars.RegisterHelper("myHelper", (writer, context, args) => {
                var count = 0;
                foreach (var arg in args)
                {
                    writer.Write("\nThing {0}: {1}", ++count, arg);
                }
            });

            var source = "Here are some things: {{myHelper args.[0].arg args.[1].arg 'another argument'}}";

            var template = Handlebars.Compile(source);

            var data = new
            {
                args = new[] { new { arg = "foo" }, new { arg = "bar" } }
            };

            var output = template(data);

            var expected = "Here are some things: \nThing 1: foo\nThing 2: bar\nThing 3: another argument";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void EmptyBlockHelperWithInversion()
        {
            var source = "{{#ifCond}}{{else}}Inverse{{/ifCond}}";

            Handlebars.RegisterHelper("ifCond", (writer, options, context, arguments) => {
                options.Inverse(writer, (object)context);
            });

            var data = new
            {
            };

            var template = Handlebars.Compile(source);

            var output = template(data);
            Assert.Equal("Inverse", output);
        }
    }
}

