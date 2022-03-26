using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.IO;
using System.Linq;
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
        
        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/422
        [Fact]
        public void CallPartialInEach()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterTemplate("testPartial", " 42 ");
            var source = "{{#each Fruits}}{{> testPartial aPartialParameter=\"couldBeAnything\"}}{{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                Fruits = new[] {"apple", "banana" }
            };
            
            var actual = template(data);
            var expected = " 42  42 ";
            
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

        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/300
        [Fact]
        public void PartialLayoutAndInlineBlock()
        {
            string layout = "{{#>body}}{{fallback}}{{/body}}";
            string page = @"{{#>layout}}{{#*inline ""body""}}{{truebody}}{{/inline}}{{/body}}{{/layout}}";

            var handlebars = Handlebars.Create();
            var template = handlebars.Compile(page);

            var data = new
            {
                fallback = "aaa",
                truebody = "Hello world"
            };

            using (var reader = new StringReader(layout))
            {
                var partialTemplate = handlebars.Compile(reader);
                handlebars.RegisterTemplate("layout", partialTemplate);
            }
            
            var result = template(data);
            Assert.Equal("Hello world", result);
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
        
        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/408
        [Theory]
        [ClassData(typeof(CollectionsOutOfRangeGenerator))]
        public void Empty_string_if_index_is_out_of_range(IEnumerable input)
        {
            var handlebars = Handlebars.Create();
            var render = handlebars.Compile("{{input.[1]}}");
            object data = new { input };

            var actual = render(data);
            Assert.Equal("", actual);
        }

        private class EscapeExpressionGenerator : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                const string OKAY = "OKAY";
                var chars = " !\"#%&'()*+,./;<=>@[\\]^`{|}~".ToCharArray();
                var words = new[] { "true", "false","null", "undefined", "foo" };

                var testList = new List<string>();
                var theContext = new Dictionary<string, object>();
                foreach (var word in words)
                {
                    foreach (var @char in chars)
                    {
                        //You can't use the same identifier you use for literal notation.
                        if (@char == '[' || @char == ']') continue;
                            
                        var theKeyWord = $"{word}{@char}{word}";
                        var testSegment = $"[{theKeyWord}]";
                        testList.Add(testSegment);
                        theContext[theKeyWord]=OKAY;
                    }
                }

                for (var index = 0; index < testList.Count; index++)
                {
                    yield return new object[] { $"{{{{ { testList[index] } }}}}", OKAY, theContext };
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class CollectionsOutOfRangeGenerator : IEnumerable<object[]>
        {
            private readonly List<IEnumerable> _data = new List<IEnumerable>
            {
                new[] { "one" },
                new List<string>{ "one" },
                new HashSet<string>{ "one" },
                new CustomReadOnlyList(new[] { "one" }),
                Enumerable.Range(0, 1).Select(i => "one")
            };

            public IEnumerator<object[]> GetEnumerator() => _data.Select(o => new object[] { o }).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            
            private class CustomReadOnlyList : IReadOnlyList<string>
            {
                private readonly IReadOnlyList<string> _readOnlyListImplementation;

                public CustomReadOnlyList(IReadOnlyList<string> readOnlyListImplementation)
                {
                    _readOnlyListImplementation = readOnlyListImplementation;
                }

                public IEnumerator<string> GetEnumerator()
                {
                    return _readOnlyListImplementation.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return ((IEnumerable) _readOnlyListImplementation).GetEnumerator();
                }

                public int Count => _readOnlyListImplementation.Count;

                public string this[int index] => _readOnlyListImplementation[index];
            }
        }

        // issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/412
        [Fact]
        public void StructReflectionAccessor()
        {
            const string template = "{{Name}}";

            var handlebars = Handlebars.Create();
            var handlebarsTemplate = handlebars.Compile(template);

            var actual = handlebarsTemplate(new CustomStruct
            {
                Name = "Foo"
            });

            Assert.Equal("Foo", actual);
        }

        private struct CustomStruct
        {
            public string Name { get; set; }
        }
        
        // Issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/432
        [Fact]
        public void WeirdBehaviour()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterTemplate("displayListItem", "{{this}},");
            handlebars.RegisterTemplate("displayList", "{{#each this}}{{> displayListItem}}{{/each}}");
            
            var template = handlebars.Compile("{{> displayList TheList}}");
            var actual1 = template(new ClassWithAList());
            var actual2 = template(new ClassWithAListAndOtherMembers());
            var expected = "";

            Assert.Equal(expected, actual1);
            Assert.Equal(expected, actual2);
        }

        // Issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/457
        [Fact]
        public void StringLength()
        {
            var handlebars = Handlebars.Create();
            var render = handlebars.Compile("{{str.length}}");
            object data = new { str = "string" };

            var actual = render(data);
            Assert.Equal("6", actual);
        }

        // Issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/470
        [Theory]
        [ClassData(typeof(EscapeExpressionGenerator))]
        public void SegmentLiteralNotationTest(string template, string expected, Dictionary<string, object> context)
        {
            var handlebars = Handlebars.Create();
            var renderer = handlebars.Compile(template);

            var actual = renderer(context);
            
            Assert.Equal(expected, actual);
        }
        
        private class ClassWithAList
        {
            public IEnumerable<string> TheList { get; set; }
        }

        private class ClassWithAListAndOtherMembers
        {
            public IEnumerable<string> TheList { get; set; }
            public bool SomeBool { get; set; }
            public string SomeString { get; set; } = "I shouldn't show up!";
        }

        // Issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/468
        // Issue refers to chinese characters, bug tested here affects any
        // html char and any non-ascii character.
        [Theory]
        [InlineData(true, "<", "<")]
        [InlineData(false, "<", "&lt;")]
        public void ConfigNoEscapeHtmlCharsShouldNotBeEscapedAfterWritingTripleCurlyValue(bool noEscape, string inputChar, string expectedChar)
        {
            // Affects any value written after execution of expression built in
            // class UnencodedStatementVisitor when config NoEscape is true.
            // Using triple curly brackets to trigger this case.

            var template = "{{{ArbitraryText}}} {{HtmlSymbol}}";
            var value = new
            {
                ArbitraryText = "text",
                HtmlSymbol = inputChar
            };

            var expected = $"text {expectedChar}";

            var config = new HandlebarsConfiguration
            {
                NoEscape = noEscape
            };
            var actual = Handlebars.Create(config).Compile(template).Invoke(value);

            Assert.Equal(expected, actual);
        }

        // Issue: https://github.com/Handlebars-Net/Handlebars.Net/issues/500
        // Issue refers to the last letter being cut off when using 
        // keys set in context
        [Fact]
        public void LastLetterCutOff()
        {
            var context = ImmutableDictionary<string, object>.Empty
                .Add("Name", "abcd");

            var template = "{{.Name}}";
            var compiledTemplate = Handlebars.Compile(template);
            string templateOutput = compiledTemplate(context);

            Assert.Equal("abcd", templateOutput);
        }
    }
}