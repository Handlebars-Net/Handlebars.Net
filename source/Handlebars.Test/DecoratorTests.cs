using System.Collections.Generic;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.ValueProviders;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class DecoratorTests
    {
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDecorator(IHandlebars handlebars)
        {
            string source = "{{#block @value-from-decorator}}{{*decorator 42}}{{@value}}{{/block}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator", 
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
            {
                options.Data.CreateProperty("value-from-decorator", arguments[0], out _);

                return function;
            });
            
            var template = handlebars.Compile(source);
            
            var result = template(null);
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void OverrideFunctionWithDecorator(IHandlebars handlebars)
        {
            string source = "{{#block}}{{*decorator 4}}2{{/block}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator", 
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    var value = arguments.At<int>(0); 
                    return (in EncodedTextWriter writer, BindingContext bindingContext) =>
                    {
                        writer.WriteSafeString(value);
                        function(writer, bindingContext);
                    };
                });
            
            var template = handlebars.Compile(source);
            
            var result = template(null);
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void RegisterMethodFromDecorator(IHandlebars handlebars)
        {
            string source = "{{*decorator 42}}{{#block}}{{method-from-decorator 1}}{{/block}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator", 
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    var value = arguments.At<int>(0);
                    options.RegisterHelper("method-from-decorator", (c, a) => value * a.At<int>(0));
                    
                    return function;
                });
            
            var template = handlebars.Compile(source);
            
            var result = template(null);
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicLateDecorator(IHandlebars handlebars)
        {
            string source = "{{#block @value-from-decorator}}{{*decorator 42}}{{@value}}{{/block}}";
            
            var template = handlebars.Compile(source);
            
            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator", 
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", arguments[0], out _);

                    return function;
                });
            
            var result = template(null);
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DecoratorInCorrectContext(IHandlebars handlebars)
        {
            string source = "{{#with inner}}{{*decorator outer}}{{#block @value-from-decorator}}{{@value}}{{/block}}{{/with}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator", 
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", arguments[0], out _);

                    return function;
                });
            
            var template = handlebars.Compile(source);
            
            var result = template(new
            {
                outer = 42,
                inner = 24
            });
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicBlockDecorator(IHandlebars handlebars)
        {
            string source = "{{#block @value-from-decorator}}{{#*decorator}}42{{/decorator}}{{@value}}{{/block}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator",
                (TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", options.Template(), out _);

                    return function;
                });
            
            var template = handlebars.Compile(source);
            
            var result = template(null);
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void InnerDecorator(IHandlebars handlebars)
        {
            string source = "{{#block @value-from-decorator}}{{#*decorator}}{{*decorator1 42}}{{/decorator}}{{@value}}{{/block}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator",
                (TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", options.Data["value-from-inner-decorator"], out _);

                    return function;
                });
            
            handlebars.RegisterDecorator("decorator1",
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-inner-decorator", arguments.At<int>(0), out _);

                    return function;
                });
            
            var template = handlebars.Compile(source);
            
            var result = template(null);
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void InnerBlockDecorator(IHandlebars handlebars)
        {
            string source = "{{#block @value-from-decorator}}{{#*decorator}}{{#*decorator1}}42{{/decorator1}}{{/decorator}}{{@value}}{{/block}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator",
                (TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", options.Data["value-from-inner-decorator"], out _);
                });
            
            handlebars.RegisterDecorator("decorator1",
                (TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-inner-decorator", options.Template(), out _);
                });
            
            var template = handlebars.Compile(source);
            
            var result = template(null);
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicLateBlockDecorator(IHandlebars handlebars)
        {
            string source = "{{#block @value-from-decorator}}{{#*decorator}}42{{/decorator}}{{@value}}{{/block}}";
            
            var template = handlebars.Compile(source);
            
            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator",
                (TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", options.Template(), out _);
                    
                    return function;
                });
            
            var result = template(null);
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicBlockDecoratorWithBlockParams(IHandlebars handlebars)
        {
            string source = "{{#block @value-from-decorator}}{{#*decorator as |value-from-decorator| }}42{{/decorator}}{{@value}}{{/block}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator", 
                (TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments) =>
            {
                var blockParamsValues = new BlockParamsValues(options.Frame, options.BlockVariables);
                blockParamsValues[0] = options.Template();

                return function;
            });
            
            var template = handlebars.Compile(source);
            
            var result = template(null);
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicBlockDecoratorWithParameter(IHandlebars handlebars)
        {
            string source = "{{#block @value-from-decorator}}{{#*decorator outer as |value-from-decorator| }}2{{/decorator}}{{@value}}{{/block}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator", 
                (TemplateDelegate _, in BlockDecoratorOptions options, in Context context, in Arguments arguments) =>
            {
                var blockParamsValues = new BlockParamsValues(options.Frame, options.BlockVariables);
                blockParamsValues[0] = $"{arguments.At<int>(0)}{options.Template()}";
            });
            
            var template = handlebars.Compile(source);
            
            var result = template(new { outer = 4});
            Assert.Equal("42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BlockDecoratorInCorrectContext(IHandlebars handlebars)
        {
            string source = "{{#with inner}}{{#*decorator}}{{outer}}{{/decorator}}{{#block @value-from-decorator}}{{@value}}{{/block}}{{/with}}";

            handlebars.RegisterHelper("block", (output, options, context, arguments) =>
            {
                options.Data.CreateProperty("value", arguments[0], out _);
                options.Template(output, context);
            });
            
            handlebars.RegisterDecorator("decorator", 
                (TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments) =>
            {
                options.Data.CreateProperty("value-from-decorator", options.Template(), out _);
            });
            
            var template = handlebars.Compile(source);
            
            var result = template(new
            {
                outer = 42,
                inner = 24
            });
            Assert.Equal("42", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DecoratorInIterator(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{*decorator 42}}{{@value-from-decorator}}-{{this}} {{/each}}";
            handlebars.RegisterDecorator("decorator",
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", arguments[0], out _);
                });
            
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new Dictionary<string, object>
                {
                    { "foo", "hello" },
                    { "bar", "world" }
                }
            };
            var result = template(data);
            Assert.Equal("42-hello 42-world ", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BlockDecoratorInIterator(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{#*decorator}}42{{/decorator}}{{@value-from-decorator}}-{{this}} {{/each}}";
            handlebars.RegisterDecorator("decorator",
                (TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", options.Template(), out _);
                });
            
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new Dictionary<string, object>
                {
                    { "foo", "hello" },
                    { "bar", "world" }
                }
            };
            var result = template(data);
            Assert.Equal("42-hello 42-world ", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DecoratorInCondition(IHandlebars handlebars)
        {
            string source = "{{#if @value-from-decorator}}{{*decorator truthy}}{{@value-from-decorator}} is Truthy!{{/if}}";

            handlebars.RegisterDecorator("decorator",
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", arguments[0], out _);
                });
            
            var template = handlebars.Compile(source);

            var data = new
            {
                truthy = 1
            };

            var result = template(data);
            Assert.Equal("1 is Truthy!", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DecoratorInDeferredBlockString(IHandlebars handlebars)
        {
            string source = "{{#person}}{{*decorator this.person}}{{@value-from-decorator}} is {{this}}{{/person}}";

            handlebars.RegisterDecorator("decorator",
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", arguments[0], out _);
                });
            
            var template = handlebars.Compile(source);

            var result = template(new { person = "Bill" });
            Assert.Equal("Bill is Bill", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DecoratorInDeferredBlockEnumerable(IHandlebars handlebars)
        {
            string source = "{{#people}}{{*decorator this.outer}}{{@value-from-decorator}}->{{this}} {{/people}}";

            handlebars.RegisterDecorator("decorator",
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) =>
                {
                    options.Data.CreateProperty("value-from-decorator", arguments[0], out _);
                });
            
            var template = handlebars.Compile(source);

            var data = new
            {
                outer = 42,
                people = new[] {
                    "Bill",
                    "Mary"
                }
            };

            var result = template(data);
            Assert.Equal("42->Bill 42->Mary ", result);
        }
    }
}