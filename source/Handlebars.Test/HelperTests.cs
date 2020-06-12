using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Features;

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
        public void BlockHelperWithBlockParams()
        {
            Handlebars.RegisterHelper("myHelper", (writer, options, context, args) => {
                var count = 0;
                options.BlockParams((parameters, binder, deps) =>
                {
                    binder(parameters.ElementAtOrDefault(0), ctx => ++count);
                });
                
                foreach(var arg in args)
                {
                    options.Template(writer, arg);
                }
            });

            var source = "Here are some things: {{#myHelper 'foo' 'bar' as |counter|}}{{counter}}:{{this}}\n{{/myHelper}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Here are some things: 1:foo\n2:bar\n";

            Assert.Equal(expected, output);
        }
        
        [Fact]
        public void BlockHelperLateBound()
        {
            var source = "Here are some things: \n" +
                         "{{#myHelper 'foo' 'bar' as |counter|}}\n" +
                         "{{counter}}:{{this}}\n" +
                         "{{/myHelper}}";

            var template = Handlebars.Compile(source);

            Handlebars.RegisterHelper("myHelper", (writer, options, context, args) => {
                var count = 0;
                options.BlockParams((parameters, binder, deps) => 
                    binder(parameters.ElementAtOrDefault(0), ctx => ++count));
                
                foreach(var arg in args)
                {
                    options.Template(writer, arg);
                }
            });
            
            var output = template(new { });

            var expected = "Here are some things: \n1:foo\n2:bar\n";

            Assert.Equal(expected, output);
        }
        
        [Fact]
        public void BlockHelperLateBoundConflictsWithValue()
        {
            var source = "{{#late}}late{{/late}}";

            var handlebars = Handlebars.Create();
            var template = handlebars.Compile(source);

            handlebars.RegisterHelper("late", (writer, options, context, args) =>
            {
                options.Template(writer, context);
            });
            
            var output = template(new { late = "should be ignored" });

            var expected = "late";

            Assert.Equal(expected, output);
        }
        
        [Fact]
        public void BlockHelperLateBoundMissingHelperFallbackToDeferredSection()
        {
            var source = "{{#late}}late{{/late}}";

            var handlebars = Handlebars.Create();
            handlebars.Configuration.RegisterMissingHelperHook(
                (context, arguments) => "Hook"
            );
            var template = handlebars.Compile(source);
            
            var output = template(new { late = "late" });

            var expected = "late";

            Assert.Equal(expected, output);
        }
        
        [Fact]
        public void HelperLateBound()
        {
            var source = "{{lateHelper}}";

            var template = Handlebars.Compile(source);

            var expected = "late";
            Handlebars.RegisterHelper("lateHelper", (writer, context, arguments) =>
            {
                writer.WriteSafeString(expected);
            });
            
            var output = template(null);

            Assert.Equal(expected, output);
        }
        
        [Theory]
        [InlineData("[$lateHelper]")]
        [InlineData("[late.Helper]")]
        [InlineData("[@lateHelper]")]
        public void HelperEscapedLateBound(string helperName)
        {
            var handlebars = Handlebars.Create();

            var source = "{{" + helperName + "}}";

            var template = handlebars.Compile(source);

            var expected = "late";
            handlebars.RegisterHelper(helperName.Trim('[', ']'), (writer, context, arguments) =>
            {
                writer.WriteSafeString(expected);
            });
            
            var output = template(null);

            Assert.Equal(expected, output);
        }
        
        [Theory]
        [InlineData("{{lateHelper.a}}")]
        [InlineData("{{[lateHelper].a}}")]
        [InlineData("{{[lateHelper.a]}}")]
        public void WrongHelperLiteralLateBound(string source)
        {
            var handlebars = Handlebars.Create();

            var template = handlebars.Compile(source);
            
            handlebars.RegisterHelper("lateHelper", (writer, context, arguments) =>
            {
                writer.WriteSafeString("should not appear");
            });
            
            var output = template(null);

            Assert.Equal(string.Empty, output);
        }
        
        [Theory]
        [InlineData("missing")]
        [InlineData("[missing]")]
        [InlineData("[$missing]")]
        [InlineData("[m.i.s.s.i.n.g]")]
        public void MissingHelperHook(string helperName)
        {
            var handlebars = Handlebars.Create();
            var format = "Missing helper: {0}";
            handlebars.Configuration
                .RegisterMissingHelperHook(
                    (context, arguments) =>
                    {
                        var name = arguments.Last().ToString();
                        return string.Format(format, name.Trim('[', ']'));
                    });

            var source = "{{"+ helperName +"}}";

            var template = handlebars.Compile(source);
            
            var output = template(null);

            var expected = string.Format(format, helperName.Trim('[', ']'));
            Assert.Equal(expected, output);
        }
        
        [Fact]
        public void MissingHelperHookViaFeatureAndMethod()
        {
            var expected = "Hook";
            var handlebars = Handlebars.Create();
            handlebars.Configuration
                .RegisterMissingHelperHook(
                    (context, arguments) => expected
                );

            handlebars.RegisterHelper("helperMissing", 
                (context, arguments) => "Should be ignored"
            );
            
            var source = "{{missing}}";
            var template = handlebars.Compile(source);
            
            var output = template(null);
            
            Assert.Equal(expected, output);
        }
        
        [Theory]
        [InlineData("missing")]
        [InlineData("[missing]")]
        [InlineData("[$missing]")]
        [InlineData("[m.i.s.s.i.n.g]")]
        public void MissingHelperHookViaHelperRegistration(string helperName)
        {
            var handlebars = Handlebars.Create();
            var format = "Missing helper: {0}";
            handlebars.RegisterHelper("helperMissing", (context, arguments) =>
            {
                var name = arguments.Last().ToString();
                return string.Format(format, name.Trim('[', ']'));
            });

            var source = "{{"+ helperName +"}}";

            var template = handlebars.Compile(source);
            
            var output = template(null);

            var expected = string.Format(format, helperName.Trim('[', ']'));
            Assert.Equal(expected, output);
        }
        
        [Theory]
        [InlineData("missing")]
        [InlineData("[missing]")]
        [InlineData("[$missing]")]
        [InlineData("[m.i.s.s.i.n.g]")]
        public void MissingBlockHelperHook(string helperName)
        {
            var handlebars = Handlebars.Create();
            var format = "Missing block helper: {0}";
            handlebars.Configuration
                .RegisterMissingHelperHook(
                    blockHelperMissing: (writer, options, context, arguments) =>
                    {
                        var name = options.GetValue<string>("name");
                        writer.WriteSafeString(string.Format(format, name.Trim('[', ']')));
                    });

            var source = "{{#"+ helperName +"}}should not appear{{/" + helperName + "}}";

            var template = handlebars.Compile(source);
            
            var output = template(null);

            var expected = string.Format(format, helperName.Trim('[', ']'));
            Assert.Equal(expected, output);
        }
        
        [Theory]
        [InlineData("missing")]
        [InlineData("[missing]")]
        [InlineData("[$missing]")]
        [InlineData("[m.i.s.s.i.n.g]")]
        public void MissingBlockHelperHookViaHelperRegistration(string helperName)
        {
            var handlebars = Handlebars.Create();
            var format = "Missing block helper: {0}";
            handlebars.RegisterHelper("blockHelperMissing", (writer, options, context, arguments) =>
            {
                var name = options.GetValue<string>("name");
                writer.WriteSafeString(string.Format(format, name.Trim('[', ']')));
            });

            var source = "{{#"+ helperName +"}}should not appear{{/" + helperName + "}}";

            var template = handlebars.Compile(source);
            
            var output = template(null);

            var expected = string.Format(format, helperName.Trim('[', ']'));
            Assert.Equal(expected, output);
        }
        
        [Fact]
        public void MissingHelperHookWhenVariableExists()
        {
            var handlebars = Handlebars.Create();
            var expected = "Variable";
            
            handlebars.Configuration
                .RegisterMissingHelperHook(
                    (context, arguments) => "Hook"
                );

            var source = "{{missing}}";

            var template = Handlebars.Compile(source);
            
            var output = template(new { missing = "Variable" });
            
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
        public void BlockHelperWithArbitraryInversionAndComplexOperator()
        {
            Handlebars.RegisterHelper("ifCond", (writer, options, context, args) => {
                if (args.Length != 3)
                {
                    writer.Write("ifCond:Wrong number of arguments");
                    return;
                }
                if (args[0] == null || args[0].GetType().Name == "UndefinedBindingResult")
                {
                    writer.Write("ifCond:args[0] undefined");
                    return;
                }
                if (args[1] == null || args[1].GetType().Name == "UndefinedBindingResult")
                {
                    writer.Write("ifCond:args[1] undefined");
                    return;
                }
                if (args[2] == null || args[2].GetType().Name == "UndefinedBindingResult")
                {
                    writer.Write("ifCond:args[2] undefined");
                    return;
                }
                if (args[0].GetType().Name == "String" || args[0].GetType().Name == "JValue")
                {
                    var val1 = args[0].ToString();
                    var val2 = args[2].ToString();

                    switch (args[1].ToString())
                    {
                        case ">":
                            if (val1.Length > val2.Length)
                            {
                                options.Template(writer, (object)context);
                            }
                            else
                            {
                                options.Inverse(writer, (object)context);
                            }
                            break;
                        case "=":
                        case "==":
                            if (val1 == val2)
                            {
                                options.Template(writer, (object)context);
                            }
                            else
                            {
                                options.Inverse(writer, (object)context);
                            }
                            break;
                        case "<":
                            if (val1.Length < val2.Length)
                            {
                                options.Template(writer, (object)context);
                            }
                            else
                            {
                                options.Inverse(writer, (object)context);
                            }
                            break;
                        case "!=":
                        case "<>":
                            if (val1 != val2)
                            {
                                options.Template(writer, (object)context);
                            }
                            else
                            {
                                options.Inverse(writer, (object)context);
                            }
                            break;
                    }
                }
                else
                {
                    var val1 = float.Parse(args[0].ToString());
                    var val2 = float.Parse(args[2].ToString());

                    switch (args[1].ToString())
                    {
                        case ">":
                            if (val1 > val2)
                            {
                                options.Template(writer, (object)context);
                            }
                            else
                            {
                                options.Inverse(writer, (object)context);
                            }
                            break;
                        case "=":
                        case "==":
                            if (val1 == val2)
                            {
                                options.Template(writer, (object)context);
                            }
                            else
                            {
                                options.Inverse(writer, (object)context);
                            }
                            break;
                        case "<":
                            if (val1 < val2)
                            {
                                options.Template(writer, (object)context);
                            }
                            else
                            {
                                options.Inverse(writer, (object)context);
                            }
                            break;
                        case "!=":
                        case "<>":
                            if (val1 != val2)
                            {
                                options.Template(writer, (object)context);
                            }
                            else
                            {
                                options.Inverse(writer, (object)context);
                            }
                            break;
                    }
                }
            });

            var template = Handlebars.Compile(@"{{#ifCond arg1 '>' arg2}}{{arg1}} is greater than {{arg2}}{{else}}{{arg1}} is less than {{arg2}}{{/ifCond}}");
            var data = new { arg1 = 2, arg2 = 1 };
            var result = template(data);
            Assert.Equal("2 is greater than 1", result);

            data = new { arg1 = 1, arg2 = 2 };
            result = template(data);
            Assert.Equal("1 is less than 2", result);

            template = Handlebars.Compile(@"{{#ifCond arg1 '<' arg2}}{{arg1}} is less than {{arg2}}{{else}}{{arg1}} is greater than {{arg2}}{{/ifCond}}");
            data = new { arg1 = 2, arg2 = 1 };
            result = template(data);
            Assert.Equal("2 is greater than 1", result);

            data = new { arg1 = 1, arg2 = 2 };
            result = template(data);
            Assert.Equal("1 is less than 2", result);

            template = Handlebars.Compile(@"{{#ifCond arg1 '=' arg2}}{{arg1}} is eq to {{arg2}}{{else}}{{arg1}} is not eq to {{arg2}}{{/ifCond}}");
            data = new { arg1 = 1, arg2 = 1 };
            result = template(data);
            Assert.Equal("1 is eq to 1", result);

            data = new { arg1 = 2, arg2 = 1 };
            result = template(data);
            Assert.Equal("2 is not eq to 1", result);

            template = Handlebars.Compile(@"{{#ifCond arg1 '!=' arg2}}{{arg1}} is not eq to {{arg2}}{{else}}{{arg1}} is eq to {{arg2}}{{/ifCond}}");
            data = new { arg1 = 2, arg2 = 1 };
            result = template(data);
            Assert.Equal("2 is not eq to 1", result);

            template = Handlebars.Compile(@"{{#ifCond str '!=' ''}}not empty{{else}}empty{{/ifCond}}");
            var datastr = new { str = "abc" };
            result = template(datastr);
            Assert.Equal("not empty", result);

            template = Handlebars.Compile(@"{{#ifCond str '==' ''}}empty{{else}}not empty{{/ifCond}}");
            datastr = new { str = "" };
            result = template(datastr);
            Assert.Equal("empty", result);
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
            var h = Handlebars.Create();
            h.RegisterHelper("myHelper", (writer, context, args) => {
                var hash = args[2] as Dictionary<string, object>;
                foreach(var item in hash)
                {
                    writer.Write(" {0}: {1}", item.Key, item.Value);
                }
            });

            var source = "Here are some things:{{myHelper 'foo' 'bar' item1='val1' item2='val2'}}";

            var template = h.Compile(source);

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

        [Fact]
        public void HelperWithLiteralHashValues()
        {
            var source = "{{literalHelper Bool=true Integer=1 String=\"abc\"}}";

            Handlebars.RegisterHelper("literalHelper", (writer, context, arguments) => {
                var parameters = arguments[0] as IDictionary<string, object>;
                Assert.IsType<bool>(parameters["Bool"]);
                Assert.IsType<int>(parameters["Integer"]);
                Assert.IsType<string>(parameters["String"]);
                writer.Write($"{parameters["Bool"]} {parameters["Integer"]} {parameters["String"]}");
            });

            var data = new
            {
            };

            var template = Handlebars.Compile(source);

            var output = template(data);
            Assert.Equal("True 1 abc", output);
        }

        [Fact]
        public void HelperWithLiteralValues()
        {
            var source = "{{literalHelper true 1 \"abc\"}}";

            Handlebars.RegisterHelper("literalHelper", (writer, context, arguments) => {
                Assert.IsType<bool>(arguments[0]);
                Assert.IsType<int>(arguments[1]);
                Assert.IsType<string>(arguments[2]);
                writer.Write($"{arguments[0]} {arguments[1]} {arguments[2]}");
            });

            var data = new
            {
            };

            var template = Handlebars.Compile(source);

            var output = template(data);
            Assert.Equal("True 1 abc", output);
        }
    }
}

