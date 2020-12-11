using System;
using System.Dynamic;
using System.IO;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.StringUtils;
using HandlebarsDotNet.ValueProviders;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class IssueTests
    {
        // Issue https://github.com/zjklee/Handlebars.CSharp/issues/7
        [Fact]
        public void ValueVariableShouldNotBeAccessibleFromContext()
        {
            var handlebars = Handlebars.Create();
            var render = handlebars.Compile("{{value}}");
            var output = render(new
            {
                anotherValue = "Test"
            });
            
            Assert.Equal("", output);
        }

        // Issue https://github.com/rexm/Handlebars.Net/issues/351
        [Fact]
        public void PerhapsNull()
        {
            var handlebars = Handlebars.Create();
            var render = handlebars.Compile("{{#if PerhapsNull}}It's not null{{else}}It's null{{/if}}");
            dynamic data = new ExpandoObject();
            data.PerhapsNull = null;

            var actual = render(data);
            Assert.Equal("It's null", actual);
        }

        // Issue https://github.com/rexm/Handlebars.Net/issues/350
        // the helper has priority
        // https://handlebarsjs.com/guide/expressions.html#disambiguating-helpers-calls-and-property-lookup
        [Fact]
        public void HelperWithSameNameVariable()
        {
            var handlebars = Handlebars.Create();
            var expected = "Helper";
            handlebars.RegisterHelper("foo", (context, arguments) => expected);

            var template = handlebars.Compile("{{foo}}");
            var data = new {foo = "Variable"};
            var actual = template(data);
            Assert.Equal(expected, actual);
        }

        // Issue https://github.com/rexm/Handlebars.Net/issues/350
        [Fact]
        public void LateBoundHelperWithSameNameVariable()
        {
            var handlebars = Handlebars.Create();
            var template = handlebars.Compile("{{amoeba}}");

            Assert.Equal("Variable", template(new {amoeba = "Variable"}));

            handlebars.RegisterHelper("amoeba", (writer, context, arguments) => { writer.Write("Helper"); });

            Assert.Equal("Helper", template(new {amoeba = "Variable"}));
        }
        
        // Issue https://github.com/rexm/Handlebars.Net/issues/350
        [Fact]
        public void LateBoundHelperWithSameNameVariablePath()
        {
            var handlebars = Handlebars.Create();
            var expected = "Variable";
            var template = handlebars.Compile("{{amoeba.a}}");
            var data = new {amoeba = new {a = expected}};
            
            var actual = template(data);
            Assert.Equal(expected, actual);

            handlebars.RegisterHelper("amoeba", (context, arguments) => "Helper");
            
            actual = template(data);
            Assert.Equal(expected, actual);
        }
        
        // Issue https://github.com/rexm/Handlebars.Net/issues/354
        [Fact]
        public void BlockHelperWithInversion()
        {
            string source = "{{^test input}}empty{{else}}not empty{{/test}}";

            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("test", (output, options, context, arguments) =>
            {
                if (HandlebarsUtils.IsTruthy(arguments[0]))
                {
                    options.Template(output, context);
                }
                else
                {
                    options.Inverse(output, context);
                }
            });

            var template = handlebars.Compile(source);
    
            Assert.Equal("empty", template(null));
            Assert.Equal("empty", template(new { otherInput = 1 }));
            Assert.Equal("not empty", template(new { input = 1 }));
        }

        // Issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/383
        [Fact]
        public void TestNestedPartials()
        {
            var innerPartial = @"{{#>outer-partial}}<br />
        Begin inner partial<br />
            Begin inner partial block<br />
                {{>@partial-block}}
            End  inner partial block<br />
        End inner partial<br />
        {{/outer-partial}}";

            var outerPartial = @"Begin outer partial<br />
            Begin outer partial block
                {{>@partial-block}}
            End outer partial block<br />
        End outer partial";

            var view = @"{{#>inner-partial}}
          View<br />
        {{/inner-partial}}";

            var handlebars = Handlebars.Create();
            handlebars.RegisterTemplate("outer-partial", outerPartial);
            handlebars.RegisterTemplate("inner-partial", innerPartial);

            var callback = handlebars.Compile(view);
            string result = callback(new object());

            const string expected = @"Begin outer partial<br />
            Begin outer partial block
<br />
        Begin inner partial<br />
            Begin inner partial block<br />
          View<br />
            End  inner partial block<br />
        End inner partial<br />
            End outer partial block<br />
        End outer partial";
            
            Assert.Equal(expected, result);
        }
        
        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/395
        [Fact]
        public void RenderingWithUnusedPartial()
        {
            var handlebars = Handlebars.Create();

            var mainTemplate = @"
{{>Navigation}}
";

            var navPartial = @"
<div>
    {{#each MenuItems}}
    <div>Menu Item: {{Name}}</div>
    {{/each}}
</div>";

            // Remove the {{#if First}} section, and the test will pass
            var unusedPartial = @"
{{#each Results}}
    Result
    {{#if HasSinglePackage}}
        HasSinglePackage
        {{#if First}}
            First
        {{/if}}
    {{/if}}
{{/each}}";

            var navTemplate = handlebars.Compile(mainTemplate);

            using (var reader = new StringReader(navPartial))
            {
                handlebars.RegisterTemplate("Navigation", handlebars.Compile(reader));
            }

            // Comment this section out and the test will succeed
            using (var reader = new StringReader(unusedPartial))
            {
                handlebars.RegisterTemplate("Unused", handlebars.Compile(reader));
            }

            // Attempted with concrete classes, still fails
            var context = new
            {
                MenuItems = new[]
                {
                    new { Name = "Getting Started"}
                }
            };

            var transformed = navTemplate(context).Trim();

            Assert.Equal(@"<div>
    <div>Menu Item: Getting Started</div>
</div>", transformed);
        }

        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/394
        [Fact]
        public void SlashesInTemplateKey()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterTemplate("Foo/bar", "hello world");
            var compiled = handlebars.Compile("{{#>Foo/bar }} {{/Foo/bar}}");
            var result = compiled(null);

            Assert.Equal("hello world", result);
        }
        
        
        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/399
        [Fact]
        public void PassesWhenMemberAccessorIsNull()
        {
            var template = "{{Join ['a', \"b \",  42] ':'}}";

            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper(new JoinHelper());

            var handlebarsTemplate = handlebars.Compile(template);
            var actual = handlebarsTemplate("");
            
            Assert.Equal("a:b :42", actual);
        }
        
        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/387
        [Fact]
        public void ReverseIfCondition()
        {
            const string template = "{{^if false}}false{{/if}}";

            var handlebars = Handlebars.Create();
            var handlebarsTemplate = handlebars.Compile(template);
            var actual = handlebarsTemplate(null);
    
            Assert.Equal("false", actual);
        }
        
        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/387
        [Fact]
        public void ReverseUnlessCondition()
        {
            const string template = "{{^unless true}}false{{/unless}}";

            var handlebars = Handlebars.Create();
            var handlebarsTemplate = handlebars.Compile(template);
            var actual = handlebarsTemplate(null);
    
            Assert.Equal("false", actual);
        }
        
        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/387
        [Fact]
        public void ReverseEach()
        {
            const string template = "{{^each this}}false{{else}}{{@value}}{{/each}}";

            var handlebars = Handlebars.Create();
            var handlebarsTemplate = handlebars.Compile(template);
            var actual = handlebarsTemplate(new[]{ 1, 2 });
    
            Assert.Equal("12", actual);
            
            actual = handlebarsTemplate(null);
    
            Assert.Equal("false", actual);
        }

        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/402
        [Fact]
        public void NestedObjectIteration()
        {
            const string template = @"
                {{#with test}}

					{{#if complexItems}}
					<ul>
						{{#each complexItems}}
							<li>{{name}} {{value}} {{evenMoreComplex.name}} {{evenMoreComplex.abbr}}</li>
						{{/each}}
					</ul>
					{{/if}}

				{{/with}}";
            
            var data = new
            {
                test = new
                {
                    complexItems = new[] {
                        new { name = "a", value = 1, evenMoreComplex = new { name = "zzz", abbr = "z" } },
                        new { name = "b", value = 2, evenMoreComplex = new { name = "yyy", abbr = "y" } },
                        new { name = "c", value = 3, evenMoreComplex = new { name = "xxx", abbr = "x" } }
                    },
                }
            };

            var handlebars = Handlebars.Create();
            var handlebarsTemplate = handlebars.Compile(template);

            var result = handlebarsTemplate(data);
            
            const string expected = "<ul><li>a 1 zzz z</li><li>b 2 yyy y</li><li>c 3 xxx x</li></ul>";
            var actual = result
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\t", string.Empty);
            
            Assert.Equal(expected, actual);
        }
        
        private class JoinHelper : IHelperDescriptor<HelperOptions>
        {
            public PathInfo Name { get; } = "join";
            
            public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
            {
                return this.ReturnInvoke(options, context, arguments);
            }

            public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
            {
                var undefinedBindingResult = arguments.At<UndefinedBindingResult>(0);
                var separator = arguments.At<string>(1);
                var values = Substring.TrimStart(undefinedBindingResult.Value, '[');
                values = Substring.TrimEnd(values, ']');
                var substrings = Substring.Split(values, ',');
                var extendedEnumerator = ExtendedEnumerator<Substring>.Create(substrings);
                
                while (extendedEnumerator.MoveNext())
                {
                    var substring = extendedEnumerator.Current.Value;
                    substring = Substring.Trim(substring, ' ');
                    substring = Substring.Trim(substring, '"');
                    substring = Substring.Trim(substring, '\'');
                    output.Write(substring);
                    if (!extendedEnumerator.Current.IsLast)
                    {
                        output.Write(separator);
                    }
                }
            }
        }

        // discussion: https://github.com/Handlebars-Net/Handlebars.Net/discussions/404
        [Fact]
        public void SwitchCaseTest()
        {
            const string template = @"
                {{~#switch propertyValue}}
                    {{~#case 'a'}}a == {{@switchValue}}{{/case~}}
                    {{~#case 'b'}}b == {{@switchValue}}{{/case~}}
                    {{~#default}}the value is not provided{{/default~}}
                {{/switch~}}";

            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper(new CaseHelper());
            handlebars.RegisterHelper(new SwitchHelper());
            handlebars.RegisterHelper(new DefaultCaseHelper());

            var handlebarsTemplate = handlebars.Compile(template);

            var a = handlebarsTemplate(new {propertyValue = "a"});
            var b = handlebarsTemplate(new {propertyValue = "b"});
            var c = handlebarsTemplate(new {propertyValue = "c"});
            
            Assert.Equal("a == a", a);
            Assert.Equal("b == b", b);
            Assert.Equal("the value is not provided", c);
        }

        private class SwitchHelper : IHelperDescriptor<BlockHelperOptions>
        {
            public PathInfo Name { get; } = "switch";
            
            public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
            {
                return this.ReturnInvoke(options, context, arguments);
            }

            public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
            {
                var switchFrame = options.CreateFrame();
                var data = new DataValues(switchFrame);
                data["switchValue"] = arguments[0];
                data["__switchBlock"] = BoxedValues.True;
                data["__switchCaseMatched"] = BoxedValues.False;
                options.Template(output, switchFrame);
            }
        }
        
        private class CaseHelper : IHelperDescriptor<BlockHelperOptions>
        {
            public PathInfo Name { get; } = "case";
            
            public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
            {
                return this.ReturnInvoke(options, context, arguments);
            }

            public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
            {
                if (!(bool) options.Data["__switchBlock"]) throw new InvalidOperationException();
                if((bool) options.Data["__switchCaseMatched"]) return;
                
                var value = options.Data["switchValue"];
                if(!Equals(value, arguments[0])) return;
                var data = new DataValues(options.Frame);
                data["__switchCaseMatched"] = BoxedValues.True;
                
                options.Template(output, options.Frame); // execute `case` in switch context
            }
        }
        
        private class DefaultCaseHelper : IHelperDescriptor<BlockHelperOptions>
        {
            public PathInfo Name { get; } = "default";
            
            public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
            {
                return this.ReturnInvoke(options, context, arguments);
            }

            public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
            {
                if (!(bool) options.Data["__switchBlock"]) throw new InvalidOperationException();
                if((bool) options.Data["__switchCaseMatched"]) return;
                
                options.Template(output, options.Frame);  // execute `default` in switch context
            }
        }
    }
}