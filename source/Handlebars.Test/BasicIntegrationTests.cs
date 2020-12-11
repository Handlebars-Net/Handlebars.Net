using Xunit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Test
{
    public class HandlebarsEnvGenerator : IEnumerable<object[]>
    {
        private readonly List<IHandlebars> _data = new List<IHandlebars>
        {
            Handlebars.Create(),
            Handlebars.Create(new HandlebarsConfiguration().Configure(o => o.Compatibility.RelaxedHelperNaming = true)),
            Handlebars.Create(new HandlebarsConfiguration().UseWarmUp(types =>
            {
                types.Add(typeof(Dictionary<string, object>));
                types.Add(typeof(Dictionary<int, object>));
                types.Add(typeof(Dictionary<long, object>));
                types.Add(typeof(Dictionary<string, string>));
            })),
        };

        public IEnumerator<object[]> GetEnumerator() => _data.Select(o => new object[] { o }).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    
    public class BasicIntegrationTests
    {
        [Theory]
        [ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicEnumerableFormatter(IHandlebars handlebars)
        {
            var source = "{{values}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                values = new object[]
                {
                    "a", 'b', 1, 1L, 1F, 1D, (decimal)1, (short)1, (ushort)1, (ulong)1, (uint)1, true, false
                }
            };
            var result = template(data);
            var expected = string.Join(",", data.values);
            Assert.Equal(expected, result);
        }

        [Theory]
        [ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPath(IHandlebars handlebars)
        {
            var source = "Hello, {{name}}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory]
        [ClassData(typeof(HandlebarsEnvGenerator))]
        public void EmptyIf(IHandlebars handlebars)
        {
            var source =
@"{{#if false}}
{{else}}
{{/if}}";
            var template = handlebars.Compile(source);
            var data = new
            {
            };
            var result = template(data);
            Assert.Equal(string.Empty, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathUnresolvedBindingFormatter(IHandlebars handlebars)
        {
            var source = "Hello, {{foo}}!";

            handlebars.Configuration.UnresolvedBindingFormatter = "('{0}' is undefined)";

            var template = handlebars.Compile(source);
            var data = new
            {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, ('foo' is undefined)!", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void PathUnresolvedBindingFormatter(IHandlebars handlebars)
        {
            var source = "Hello, {{foo}}!";

            handlebars.Configuration.FormatterProviders.Add(new CustomUndefinedFormatter());

            var template = handlebars.Compile(source);
            var data = new
            {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, ('foo' is undefined)!", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void CustcomDateTimeFormat(IHandlebars handlebars)
        {
            var source = "{{now}}";

            var format = "d";
            var formatter = new CustomDateTimeFormatter(format);
            handlebars.Configuration.FormatterProviders.Add(formatter);

            var template = handlebars.Compile(source);
            var data = new
            {
                now = DateTime.Now
            };
            
            var result = template(data);
            Assert.Equal(data.now.ToString(format), result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DefaultDateTimeFormat(IHandlebars handlebars)
        {
            var source = "{{time}}";
            
            var template = handlebars.Compile(source);
            var time = "2020-11-19T23:36:08.4256520Z";
            var data = new
            {
                time = DateTime.Parse(time).ToUniversalTime()
            };
            
            var result = template(data);
            Assert.Equal(time, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathThrowOnUnresolvedBindingExpression(IHandlebars handlebars)
        {
            var source = "Hello, {{foo}}!";

            handlebars.Configuration.ThrowOnUnresolvedBindingExpression = true;
            var template = handlebars.Compile(source);

            var data = new
            {
                name = "Handlebars.Net"
            };
            
            Assert.Throws<HandlebarsUndefinedBindingException>(() => template(data));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathThrowOnNestedUnresolvedBindingExpression(IHandlebars handlebars)
        {
            var source = "Hello, {{foo.bar}}!";

            handlebars.Configuration.ThrowOnUnresolvedBindingExpression = true;
            
            var template = handlebars.Compile(source);

            var data = new
            {
                foo = (object)null
            };
            var ex = Assert.Throws<HandlebarsUndefinedBindingException>(() => template(data));
            
            Assert.Equal("bar is undefined", ex.Message);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathNoThrowOnNullExpression(IHandlebars handlebars)
        {
            var source =
@"{{#if foo}}
{{foo.bar}}
{{else}}
false
{{/if}}
";
            handlebars.Configuration.ThrowOnUnresolvedBindingExpression = true;
            var template = handlebars.Compile(source);

            var data = new
            {
                foo = (string)null
            };
            var result = template(data);
            Assert.Contains("false", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void AssertHandlebarsUndefinedBindingException(IHandlebars handlebars)
        {
            var source = "Hello, {{person.firstname}} {{person.lastname}}!";

            handlebars.Configuration.ThrowOnUnresolvedBindingExpression = true;
            var template = handlebars.Compile(source);

            var data = new
            {
                person = new
                {
                    firstname = "Erik"
                }
            };

            var exception = Assert.Throws<HandlebarsUndefinedBindingException>(() => template(data));
            Assert.Equal("person.lastname", exception.Path);
            Assert.Equal("lastname", exception.MissingKey);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathWhiteSpace(IHandlebars handlebars)
        {
            var source = "Hello, {{ name }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicCurlies(IHandlebars handlebars)
        {
            var source = "Hello, {name}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, {name}!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicCurliesWithLeadingSlash(IHandlebars handlebars)
        {
            var source = "Hello, \\{name\\}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, \\{name\\}!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicCurliesWithEscapedLeadingSlash(IHandlebars handlebars)
        {
            var source = @"Hello, \\{{name}}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal(@"Hello, \Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathArray(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.[1] }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new[] { "Foo", "Handlebars.Net" }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathArrayChildPath(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.[1].name }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new[] { new { name = "Foo" }, new { name = "Handlebars.Net" } }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathArrayNoSquareBracketsChildPath(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.1.name }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new[] { new { name = "Foo" }, new { name = "Handlebars.Net" } }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathEnumerableNoSquareBracketsChildPath(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.1.name }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new[] { new { name = "skip" }, new { name = "Foo" }, new { name = "Handlebars.Net" } }.Skip(1)
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathDotBinding(IHandlebars handlebars)
        {
            var source = "{{#nestedObject}}{{.}}{{/nestedObject}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                nestedObject = "A dot goes a long way"
            };
            var result = template(data);
            Assert.Equal("A dot goes a long way", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathRelativeDotBinding(IHandlebars handlebars)
        {
            var source = "{{#nestedObject}}{{../.}}{{/nestedObject}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                nestedObject = "Relative dots, yay"
            };
            var result = template(data);
            Assert.Equal("{ nestedObject = Relative dots, yay }", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void PathRelativeBinding(IHandlebars handlebars)
        {
            var template =
                @"{{#each users}}
                    {{this/person.name/firstName}}
                    {{#with this/person.name}}
                        {{lastName}}
                        {{lookup (lookup ../this/../users @index) 'twitter'}}
                    {{/with}}
                {{/each}}";

            var handlebarsTemplate = handlebars.Compile(template);

            var data = new
            {
                users = new object[]
                {
                    new
                    {
                        person = new
                        {
                            name = new
                            {
                                firstName = "Garry",
                                lastName = "Finch"
                            }
                        },
                        jobTitle = "Front End Technical Lead",
                        twitter = "gazraa"
                    },
                    new
                    {
                        person = new
                        {
                            name = new
                            {
                                firstName = "Karen",
                                lastName = "Finch"
                            }
                        },
                        jobTitle = "Photographer",
                        twitter = "photobasics"
                    }
                }
            };

            var result = handlebarsTemplate(data);
            var actual = string.Join(" ", result.Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim(' ')));
            Assert.Equal("Garry Finch gazraa Karen Finch photobasics", actual);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPropertyOnArray(IHandlebars handlebars)
        {
            var source = "Array is {{ names.Length }} item(s) long";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new[] { new { name = "Foo" }, new { name = "Handlebars.Net" } }
            };
            var result = template(data);
            Assert.Equal("Array is 2 item(s) long", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void AliasedPropertyOnArray(IHandlebars handlebars)
        {
            var source = "Array is {{ names.count }} item(s) long";
            handlebars.Configuration.UseCollectionMemberAliasProvider();
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new[] { new { name = "Foo" }, new { name = "Handlebars.Net" } }
            };
            var result = template(data);
            Assert.Equal("Array is 2 item(s) long", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void CustomAliasedPropertyOnArray(IHandlebars handlebars)
        {
            var aliasProvider = new DelegatedMemberAliasProvider()
                .AddAlias<IList>("myCountAlias", list => list.Count);
            
            handlebars.Configuration.AliasProviders.Add(aliasProvider);
            
            var source = "Array is {{ names.myCountAlias }} item(s) long";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new[] { new { name = "Foo" }, new { name = "Handlebars.Net" } }
            };
            var result = template(data);
            Assert.Equal("Array is 2 item(s) long", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void AliasedPropertyOnList(IHandlebars handlebars)
        {
            var source = "Array is {{ names.Length }} item(s) long";
            handlebars.Configuration.UseCollectionMemberAliasProvider();
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new List<object> { new { name = "Foo" }, new { name = "Handlebars.Net" } }
            };
            var result = template(data);
            Assert.Equal("Array is 2 item(s) long", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicIfElse(IHandlebars handlebars)
        {
            var source = "Hello, {{#if basic_bool}}Bob{{else}}Sam{{/if}}!";
            var template = handlebars.Compile(source);
            var trueData = new
            {
                basic_bool = true
            };
            var falseData = new
            {
                basic_bool = false
            };
            var resultTrue = template(trueData);
            var resultFalse = template(falseData);
            Assert.Equal("Hello, Bob!", resultTrue);
            Assert.Equal("Hello, Sam!", resultFalse);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicIfElseIf(IHandlebars handlebars)
        {
            var source = "{{#if isActive}}active{{else if isInactive}}inactive{{/if}}";
            var template = handlebars.Compile(source);
            var activeData = new
            {
                isActive = true
            };
            var inactiveData = new
            {
                isInactive = true
            };
            var resultTrue = template(activeData);
            var resultFalse = template(inactiveData);
            Assert.Equal("active", resultTrue);
            Assert.Equal("inactive", resultFalse);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicIfElseIfElse(IHandlebars handlebars)
        {
            var source = "{{#if isActive}}active{{else if isInactive}}inactive{{else}}nada{{/if}}";
            var template = handlebars.Compile(source);
            var activeData = new
            {
                isActive = true
            };
            var inactiveData = new
            {
                isInactive = true
            };
            var elseData = new
            {
            };
            var resultActive = template(activeData);
            var resultInactive = template(inactiveData);
            var resultElse = template(elseData);
            Assert.Equal("active", resultActive);
            Assert.Equal("inactive", resultInactive);
            Assert.Equal("nada", resultElse);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicWith(IHandlebars handlebars)
        {
            var source = "Hello,{{#with person}} my good friend {{name}}{{/with}}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                person = new
                {
                    name = "Erik"
                }
            };
            var result = template(data);
            Assert.Equal("Hello, my good friend Erik!", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void GlobalDataPropagation(IHandlebars handlebars)
        {
            var source = "{{#with input}}{{first}} {{@global1}} {{#with second}}{{third}} {{@global2}}{{/with}}{{/with}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                input = new
                {
                    first = 1,
                    second = new {
                        third = 3
                    }
                }
            };
            var result = template(data, new { global1 = 2, global2 = 4 });
            Assert.Equal("1 2 3 4", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void TestSingleLoopDictionary(IHandlebars handlebars)
        {
            const string source = "{{#Input}}ii={{@index}} {{/Input}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                Input = new List<string>
                {
                    "a", "b", "c"
                }
            };
            var result = template(data);
            Assert.Equal("ii=0 ii=1 ii=2 ", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void WithWithBlockParams(IHandlebars handlebars)
        {
            var source = "{{#with person as |person|}}{{person.name}} is {{age}} years old{{/with}}.";
            var template = handlebars.Compile(source);
            var data = new
            {
                person = new
                {
                    name = "Erik",
                    age = 42
                }
            };
            var result = template(data);
            Assert.Equal("Erik is 42 years old.", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicWithInversion(IHandlebars handlebars)
        {
            var source = "Hello, {{#with person}} my good friend{{else}}nevermind{{/with}}";
            var template = handlebars.Compile(source);

            Assert.Equal("Hello, nevermind", template(new { }));
            Assert.Equal("Hello, nevermind", template(new { person = false }));
            Assert.Equal("Hello, nevermind", template(new { person = new string[] { } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicEncoding(IHandlebars handlebars)
        {
            var source = "Hello, {{name}}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                name = "<b>Bob</b>"
            };
            var result = template(data);
            Assert.Equal("Hello, &lt;b&gt;Bob&lt;/b&gt;!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicComment(IHandlebars handlebars)
        {
            var source = "Hello, {{!don't render me!}}{{name}}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                name = "Carl"
            };
            var result = template(data);
            Assert.Equal("Hello, Carl!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicCommentEscaped(IHandlebars handlebars)
        {
            var source = "Hello, {{!--don't {{render}} me!--}}{{name}}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                name = "Carl"
            };
            var result = template(data);
            Assert.Equal("Hello, Carl!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicObjectEnumerator(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{this}} {{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new
                {
                    foo = "hello",
                    bar = "world"
                }
            };
            var result = template(data);
            Assert.Equal("hello world ", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicListEnumerator(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{this}} {{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new string[]
                {
                    "hello",
                    "world"
                }
            };
            var result = template(data);
            Assert.Equal("hello world ", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicObjectEnumeratorWithLast(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{@last}} {{/each}}";
            var template = handlebars.Compile(source);
            handlebars.Configuration.Compatibility.SupportLastInObjectIterations = true;
            var data = new
            {
                enumerateMe = new
                {
                    foo = "hello",
                    bar = "world"
                }
            };
            var result = template(data);
            Assert.Equal("False True ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicObjectEnumeratorWithKey(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{@key}}: {{this}} {{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new
                {
                    foo = "hello",
                    bar = "world"
                }
            };
            var result = template(data);
            Assert.Equal("foo: hello bar: world ", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ObjectEnumeratorWithBlockParams(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe as |item val|}}{{@item}}: {{@val}} {{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new
                {
                    foo = "hello",
                    bar = "world"
                }
            };
            var result = template(data);
            Assert.Equal("hello: foo world: bar ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDictionaryEnumerator(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{this}} {{/each}}";
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
            Assert.Equal("hello world ", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDictionaryEnumeratorDeep(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{this.inner.a}}{{this.inner.b}}{{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new Dictionary<string, object>
                {
                    {
                        "foo", new Dictionary<string, object>
                        {
                            ["inner"] = new Dictionary<string, object>
                            {
                                ["a"] = "1",
                                ["b"] = "2"
                            }
                        }
                    },
                    {
                        "bar", new Dictionary<string, object>
                        {
                            ["inner"] = new Dictionary<string, object>
                            {
                                ["a"] = "3",
                                ["b"] = "4"
                            }
                        }
                    }
                }
            };
            
            var result = template(data);
            Assert.Equal("1234", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DictionaryEnumeratorWithBlockParams(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe as |item val|}}{{item}} {{val}} {{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new Dictionary<string, object>
                {
                    { "foo", "hello"},
                    { "bar", "world"}
                }
            };
            var result = template(data);
            Assert.Equal("hello foo world bar ", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DictionaryWithLastEnumerator(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{@last}} {{/each}}";
            var template = handlebars.Compile(source);
            handlebars.Configuration.Compatibility.SupportLastInObjectIterations = true;
            var data = new
            {
                enumerateMe = new Dictionary<string, object>
                {
                    { "1", "1" },
                    { "2", "2" },
                    { "3", "3" }
                }
            };
            var result = template(data);
            Assert.Equal("False False True ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDictionaryEnumeratorWithIntKeys(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{this}} {{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new Dictionary<int, object>
                {
                    { 42, "hello" },
                    { 1000000017, "world" }
                }
            };
            var result = template(data);
            Assert.Equal("hello world ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDictionaryEnumeratorWithKey(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{@key}}: {{this}} {{/each}}";
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
            Assert.Equal("foo: hello bar: world ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDictionaryEnumeratorWithLongKey(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{@key}}: {{this}} {{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                enumerateMe = new Dictionary<long, object>
                {
                    { 42L, "hello" },
                    { 100000000000017L, "world" }
                }
            };
            var result = template(data);
            Assert.Equal("42: hello 100000000000017: world ", result);
        }


        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathDictionaryStringKeyNoSquareBrackets(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.Foo }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new Dictionary<string, string>
                {
                    { "Foo" , "Handlebars.Net" }
                }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathDictionaryStringKey(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.[Foo] }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new Dictionary<string, string>
                {
                    { "Foo" , "Handlebars.Net" }
                }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathDictionaryIntKeyNoSquareBrackets(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.42 }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new Dictionary<int, string>
                {
                    { 42 , "Handlebars.Net" }
                }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathDictionaryLongKeyNoSquareBrackets(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.42 }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new Dictionary<long, string>
                {
                    { 42 , "Handlebars.Net" }
                }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathDictionaryIntKey(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.[42] }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new Dictionary<int, string>
                {
                    { 42 , "Handlebars.Net" }
                }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathDictionaryLongKey(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.[42] }}!";
            var template = handlebars.Compile(source);
            var data = new
            {
                names = new Dictionary<long, string>
                {
                    { 42 , "Handlebars.Net" }
                }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathExpandoObjectIntKeyRoot(IHandlebars handlebars)
        {
            var source = "Hello, {{ [42].name }}!";
            var template = handlebars.Compile(source);
            var data = JsonConvert.DeserializeObject<ExpandoObject>("{ 42 : { \"name\": \"Handlebars.Net\" } }");

            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPathExpandoObjectIntKeyArray(IHandlebars handlebars)
        {
            var source = "Hello, {{ names.[1].name }}!";
            var template = handlebars.Compile(source);
            var data = JsonConvert.DeserializeObject<ExpandoObject>("{ names : [ { \"name\": \"nope!\" }, { \"name\": \"Handlebars.Net\" } ] }");

            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DynamicWithMetadataEnumerator(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{this}} {{/each}}";
            var template = handlebars.Compile(source);
            dynamic data = new ExpandoObject();
            data.enumerateMe = new ExpandoObject();
            data.enumerateMe.foo = "hello";
            data.enumerateMe.bar = "world";
            var result = template(data);
            Assert.Equal("hello world ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DynamicWithMetadataEnumeratorWithKey(IHandlebars handlebars)
        {
            var source = "{{#each enumerateMe}}{{@key}}: {{this}} {{/each}}";
            var template = handlebars.Compile(source);
            dynamic data = new ExpandoObject();
            data.enumerateMe = new ExpandoObject();
            data.enumerateMe.foo = "hello";
            data.enumerateMe.bar = "world";
            var result = template(data);
            Assert.Equal("foo: hello bar: world ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicHelper(IHandlebars handlebars)
        {
            handlebars.RegisterHelper("link_to", (writer, context, parameters) =>
            {
                writer.WriteSafeString("<a href='" + parameters[0] + "'>" + parameters[1] + "</a>");
            });

            string source = @"Click here: {{link_to url text}}";

            var template = handlebars.Compile(source);

            var data = new
            {
                url = "https://github.com/rexm/handlebars.net",
                text = "Handlebars.Net"
            };

            var result = template(data);
            Assert.Equal("Click here: <a href='https://github.com/rexm/handlebars.net'>Handlebars.Net</a>", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicHelperPostRegister(IHandlebars handlebars)
        {
            string source = @"Click here: {{link_to_post_reg url text}}";

            var template = handlebars.Compile(source);

            handlebars.RegisterHelper("link_to_post_reg", (writer, context, parameters) =>
            {
                writer.WriteSafeString("<a href='" + parameters[0] + "'>" + parameters[1] + "</a>");
            });

            var data = new
            {
                url = "https://github.com/rexm/handlebars.net",
                text = "Handlebars.Net"
            };

            var result = template(data);


            Assert.Equal("Click here: <a href='https://github.com/rexm/handlebars.net'>Handlebars.Net</a>", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlock(IHandlebars handlebars)
        {
            string source = "Hello, {{#person}}{{name}}{{/person}}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                person = new
                {
                    name = "Bill"
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Bill!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlockString(IHandlebars handlebars)
        {
            string source = "{{#person}} -{{this}}- {{/person}}";

            var template = handlebars.Compile(source);

            var result = template(new { person = "Bill" });
            Assert.Equal(" -Bill- ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlockWithWhitespace(IHandlebars handlebars)
        {
            string source = "Hello, {{ # person }}{{ name }}{{ / person }}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                person = new
                {
                    name = "Bill"
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Bill!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlockFalsy(IHandlebars handlebars)
        {
            string source = "Hello, {{#person}}{{name}}{{/person}}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                person = false
            };

            var result = template(data);
            Assert.Equal("Hello, !", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlockNull(IHandlebars handlebars)
        {
            string source = "Hello, {{#person}}{{name}}{{/person}}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                person = (object)null
            };

            var result = template(data);
            Assert.Equal("Hello, !", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlockEnumerable(IHandlebars handlebars)
        {
            string source = "Hello, {{#people}}{{this}} {{/people}}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                people = new[] {
                    "Bill",
                    "Mary"
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Bill Mary !", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlockNegated(IHandlebars handlebars)
        {
            string source = "Hello, {{^people}}nobody{{/people}}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                people = new string[] {
                }
            };

            var result = template(data);
            Assert.Equal("Hello, nobody!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlockNegatedContext(IHandlebars handlebars)
        {
            var template = handlebars.Compile("Hello, {{^obj}}{{name}}{{/obj}}!");

            Assert.Equal("Hello, nobody!", template(new { name = "nobody" }));
            Assert.Equal("Hello, nobody!", template(new { name = "nobody", obj = new string[0] }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlockInversion(IHandlebars handlebars)
        {
            var template = handlebars.Compile("Hello, {{#obj}}somebody{{else}}{{name}}{{/obj}}!");

            Assert.Equal("Hello, nobody!", template(new { name = "nobody" }));
            Assert.Equal("Hello, nobody!", template(new { name = "nobody", obj = false }));
            Assert.Equal("Hello, nobody!", template(new { name = "nobody", obj = new string[0] }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDeferredBlockNegatedInversion(IHandlebars handlebars)
        {
            var template = handlebars.Compile("Hello, {{^obj}}nobody{{else}}{{name}}{{/obj}}!");

            var array = new[]
            {
                new {name = "John"},
                new {name = " and "},
                new {name = "Sarah"}
            };

            Assert.Equal("Hello, John and Sarah!", template(new { obj = array }));
            Assert.Equal("Hello, somebody!", template(new { obj = true, name = "somebody" }));
            Assert.Equal("Hello, person!", template(new { obj = new { name = "person" } }));
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicPropertyMissing(IHandlebars handlebars)
        {
            string source = "Hello, {{first}} {{last}}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                first = "Marc"
            };

            var result = template(data);
            Assert.Equal("Hello, Marc !", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicNullOrMissingSubProperty(IHandlebars handlebars)
        {
            string source = "Hello, {{name.first}}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                name = (object)null
            };

            var result = template(data);
            Assert.Equal("Hello, !", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicNumericFalsy(IHandlebars handlebars)
        {
            string source = "Hello, {{#if falsy}}Truthy!{{/if}}";

            var template = handlebars.Compile(source);

            var data = new
            {
                falsy = 0
            };

            var result = template(data);
            Assert.Equal("Hello, ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicNullFalsy(IHandlebars handlebars)
        {
            string source = "Hello, {{#if falsy}}Truthy!{{/if}}";

            var template = handlebars.Compile(source);

            var data = new
            {
                falsy = (object)null
            };

            var result = template(data);
            Assert.Equal("Hello, ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicNumericTruthy(IHandlebars handlebars)
        {
            string source = "Hello, {{#if truthy}}Truthy!{{/if}}";

            var template = handlebars.Compile(source);

            var data = new
            {
                truthy = -0.1
            };

            var result = template(data);
            Assert.Equal("Hello, Truthy!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicStringFalsy(IHandlebars handlebars)
        {
            string source = "Hello, {{#if falsy}}Truthy!{{/if}}";

            var template = handlebars.Compile(source);

            var data = new
            {
                falsy = ""
            };

            var result = template(data);
            Assert.Equal("Hello, ", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicEmptyArrayFalsy(IHandlebars handlebars)
        {
            var source = "{{#if Array}}stuff: {{#each Array}}{{this}}{{/each}}{{/if}}";

            var template = handlebars.Compile(source);

            var data = new
            {
                Array = new object[] { }
            };

            var result = template(data);

            Assert.Equal("", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicTripleStash(IHandlebars handlebars)
        {
            string source = "Hello, {{{dangerous_value}}}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                dangerous_value = "<div>There's HTML here</div>"
            };

            var result = template(data);
            Assert.Equal("Hello, <div>There's HTML here</div>!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicEscape(IHandlebars handlebars)
        {
            string source = @"Hello, \{{raw_value}}!";

            var template = handlebars.Compile(source);

            var data = new
            {
                raw_value = "<div>I shouldn't display</div>"
            };

            var result = template(data);
            Assert.Equal(@"Hello, {{raw_value}}!", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicNumberLiteral(IHandlebars handlebars)
        {
            string source = "{{eval 2  3}}";

            handlebars.RegisterHelper("eval",
                (writer, context, args) => writer.Write("{0} {1}", args[0], args[1]));

            var template = handlebars.Compile(source);

            var data = new { };

            var result = template(data);
            Assert.Equal("2 3", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicNullLiteral(IHandlebars handlebars)
        {
            string source = "{{eval null}}";

            handlebars.RegisterHelper("eval",
                (writer, context, args) => writer.Write(args[0] == null));

            var template = handlebars.Compile(source);

            var data = new { };

            var result = template(data);
            Assert.Equal("True", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicCurlyBracesInLiterals(IHandlebars handlebars)
        {
            var source = @"{{verbatim '{{foo}}'}} something {{verbatim '{{bar}}'}}";

            handlebars.RegisterHelper("verbatim",
                (writer, context, args) => writer.Write(args[0]));

            var template = handlebars.Compile(source);

            var data = new { };
            var result = template(data);

            Assert.Equal("{{foo}} something {{bar}}", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicRoot(IHandlebars handlebars)
        {
            string source = "{{#people}}- {{this}} is member of {{@root.group}}\n{{/people}}";

            var template = handlebars.Compile(source);

            var data = new
            {
                group = "Engineering",
                people = new[]
                    {
                        "Rex",
                        "Todd"
                    }
            };

            var result = template(data);
            Assert.Equal("- Rex is member of Engineering\n- Todd is member of Engineering\n", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ImplicitConditionalBlock(IHandlebars handlebars)
        {
            var template =
                "{{#home}}Welcome Home{{/home}}{{^home}}Welcome to {{newCity}}{{/home}}";

            var data = new
            {
                newCity = "New York City",
                oldCity = "Los Angeles",
                home = false
            };

            var compiler = handlebars.Compile(template);
            var result = compiler.Invoke(data);
            Assert.Equal("Welcome to New York City", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicDictionary(IHandlebars handlebars)
        {
            var source =
                "<div id='userInfo'>UserName: {{userInfo.userName}} Language: {{userInfo.language}}</div>"
                + "<div id='main' style='width:{{clientSettings.width}}px; height:{{clientSettings.height}}px'>body</div>";

            var template = handlebars.Compile(source);

            var embedded = new Dictionary<string, object>
            {
                {"userInfo", new {userName = "Ondrej", language = "Slovak"}},
                {"clientSettings", new {width = 120, height = 80}}
            };

            var result = template(embedded);
            var expectedResult =
                "<div id='userInfo'>UserName: Ondrej Language: Slovak</div>"
                + "<div id='main' style='width:120px; height:80px'>body</div>";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicHashtable(IHandlebars handlebars)
        {
            var source = "{{dictionary.[key]}}";

            var template = handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new Hashtable
                {
                    { "key", "Hello world!" }
                }
            });
            var expectedResult = "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicHashtableNoSquareBrackets(IHandlebars handlebars)
        {
            var source = "{{dictionary.key}}";

            var template = handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new Hashtable
                {
                    { "key", "Hello world!" }
                }
            });
            var expectedResult = "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicMockIDictionary(IHandlebars handlebars)
        {
            var source = "{{dictionary.[key]}}";

            var template = handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new MockDictionary()
            });
            var expectedResult =
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DictionaryWithSpaceInKeyName(IHandlebars handlebars)
        {
            var source = "{{dictionary.[my key]}}";

            var template = handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new MockDictionary()
            });
            var expectedResult =
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DictionaryWithSpaceInKeyNameAndChildProperty(IHandlebars handlebars)
        {
            var source = "{{dictionary.[my key].prop1}}";

            var template = handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new Dictionary<string, object>
                    {
                        {
                            "my key", new
                            {
                                prop1 = "Hello world!"
                            }
                        }
                    }
            });
            var expectedResult =
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicMockIDictionaryNoSquareBrackets(IHandlebars handlebars)
        {
            var source = "{{dictionary.key}}";

            var template = handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new MockDictionary()
            });
            var expectedResult =
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicMockIDictionaryIntKey(IHandlebars handlebars)
        {
            var source = "{{dictionary.[42]}}";

            var template = handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new MockDictionary()
            });
            var expectedResult =
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicMockIDictionaryIntKeyNoSquareBrackets(IHandlebars handlebars)
        {
            var source = "{{dictionary.42}}";

            var template = handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new MockDictionary()
            });
            var expectedResult =
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void TestNoWhitespaceBetweenExpressions(IHandlebars handlebars)
        {

            var source = @"{{#is ProgramID """"}}no program{{/is}}{{#is ProgramID ""1081""}}some program text{{/is}}";

            handlebars.RegisterHelper("is", (output, options, context, args) =>
                {
                    if (args[0] == args[1])
                    {
                        options.Template(output, context);
                    }
                });


            var template = handlebars.Compile(source);

            var result = template(new
            {
                ProgramID = "1081"
            });

            var expectedResult =
                "some program text";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void DictionaryIteration(IHandlebars handlebars)
        {
            string source = @"{{#ADictionary}}{{@key}},{{value}}{{/ADictionary}}";
            var template = handlebars.Compile(source);
            var result = template(new
            {
                ADictionary = new Dictionary<string, int>
                        {
                            { "key5", 14 },
                            { "key6", 15 },
                            { "key7", 16 },
                            { "key8", 17 }
                        }
            });

            Assert.Equal("key5,14key6,15key7,16key8,17", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ObjectEnumeration(IHandlebars handlebars)
        {
            string source = @"{{#each myObject}}{{#if this.length}}<b>{{@key}}</b>{{#each this}}<li>{{this}}</li>{{/each}}<br>{{/if}}{{/each}}";
            var template = handlebars.Compile(source);
            var result = template(new
            {
                myObject = new
                {
                    arr = new[] { "hello", "world" },
                    notArr = 1
                }
            });

            Assert.Equal("<b>arr</b><li>hello</li><li>world</li><br>", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void NestedDictionaryWithSegmentLiteral(IHandlebars handlebars)
        {
            var source = "{{dictionary.[my key].[another key]}}";

            var template = handlebars.Compile(source);

            var data = new
            {
                dictionary =
                    new Dictionary<string, Dictionary<string, string>>()
                    {
                        {"my key", new Dictionary<string, string>() {{"another key", "Hello Dictionary!"}}}
                    }
            };

            var result = template(data);

            var expectedResult =
                "Hello Dictionary!";

            Assert.Equal(expectedResult, result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ImplicitIDictionaryImplementationShouldNotThrowNullref(IHandlebars handlebars)
        {
            // Arrange
            handlebars.RegisterHelper("foo", (writer, context, arguments) => { });
            var compile = handlebars.Compile(@"{{foo bar}}");
            var mock = new MockDictionaryImplicitlyImplemented(new Dictionary<string, object> { { "bar", 1 } });

            // Act
            compile.Invoke(mock);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ShouldBeAbleToHandleFieldContainingDots(IHandlebars handlebars) 
        { 
            var source = "Everybody was {{ foo.bar }}-{{ [foo.bar] }} {{ foo.[bar.baz].buz }}!"; 
            var template = handlebars.Compile(source); 
            var data = new Dictionary<string, object>() 
            { 
                {"foo.bar", "fu"}, 
                {"foo", new Dictionary<string,object>{{ "bar", "kung" }, { "bar.baz", new Dictionary<string, object> {{ "buz", "fighting" }} }} } 
            }; 
            var result = template(data); 
            Assert.Equal("Everybody was kung-fu fighting!", result); 
        } 
 
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ShouldBeAbleToHandleListWithNumericalFields(IHandlebars handlebars) 
        { 
            var source = "{{ [0] }}"; 
            var template = handlebars.Compile(source); 
            var data = new List<string> {"FOOBAR"}; 
            var result = template(data); 
            Assert.Equal("FOOBAR", result); 
        } 
 
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ShouldBeAbleToHandleDictionaryWithNumericalFields(IHandlebars handlebars) 
        { 
            var source = "{{ [0] }}"; 
            var template = handlebars.Compile(source); 
            var data = new Dictionary<string,string> 
            { 
                {"0", "FOOBAR"}, 
            }; 
            var result = template(data); 
            Assert.Equal("FOOBAR", result); 
        } 
 
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ShouldBeAbleToHandleJObjectsWithNumericalFields(IHandlebars handlebars) 
        { 
            var source = "{{ [0] }}"; 
            var template = handlebars.Compile(source); 
            var data = new JObject 
            { 
                {"0", "FOOBAR"}, 
            }; 
            var result = template(data); 
            Assert.Equal("FOOBAR", result); 
        } 
 
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ShouldBeAbleToHandleKeysStartingAndEndingWithSquareBrackets(IHandlebars handlebars) 
        { 
            var source = 
                "{{ noBracket }} {{ [noBracket] }} {{ [[startsWithBracket] }} {{ [endsWithBracket]] }} {{ [[bothBrackets]] }}"; 
            var template = handlebars.Compile(source); 
            var data = new Dictionary<string, string> 
            { 
                {"noBracket", "foo"}, 
                {"[startsWithBracket", "bar"}, 
                {"endsWithBracket]", "baz"}, 
                {"[bothBrackets]", "buz"} 
            }; 
            var result = template(data); 
            Assert.Equal("foo foo bar baz buz", result); 
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicReturnFromHelper(IHandlebars Handlebars)
        {
            var getData = $"getData{Guid.NewGuid()}";
            Handlebars.RegisterHelper(getData, (context, arguments) => arguments[0]);
            var source = $"{{{{{getData} 'data'}}}}";
            var template = Handlebars.Compile(source);
            
            var result = template(new object());
            Assert.Equal("data", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void CollectionReturnFromHelper(IHandlebars handlebars)
        {
            handlebars.RegisterHelper($"getData", (context, arguments) =>
            {
                var data = new Dictionary<string, string>
                {
                    {"Nils", arguments[0].ToString()},
                    {"Yehuda", arguments[1].ToString()}
                };
        
                return data;
            });
            var source = "{{#each (getData 'Darmstadt' 'San Francisco')}}{{@key}} lives in {{@value}}. {{/each}}";
            var template = handlebars.Compile(source);
            
            var result = template(new object());
            Assert.Equal("Nils lives in Darmstadt. Yehuda lives in San Francisco. ", result);
        }
        
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ReturnFromHelperWithSubExpression(IHandlebars handlebars)
        {
            var formatData = $"formatData{Guid.NewGuid()}";
            handlebars.RegisterHelper(formatData, (writer, context, arguments) =>
            {
                writer.WriteSafeString(arguments[0]);
                writer.WriteSafeString(" ");
                writer.WriteSafeString(arguments[1]);
            });
        
            var getData = $"getData{Guid.NewGuid()}";
            handlebars.RegisterHelper(getData, (context, arguments) =>
            {
                return arguments[0];
            });
            
            var source = $"{{{{{getData} ({formatData} 'data' '42')}}}}";
            var template = handlebars.Compile(source);
        
            var result = template(new object());
            Assert.Equal("data 42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void ReturnFromHelperLateBindWithSubExpression(IHandlebars handlebars)
        {
            var formatData = $"formatData{Guid.NewGuid()}";
            var getData = $"getData{Guid.NewGuid()}";
            
            var source = $"{{{{{getData} ({formatData} 'data' '42')}}}}";
            var template = handlebars.Compile(source);
            
            handlebars.RegisterHelper(formatData, (writer, context, arguments) =>
            {
                writer.WriteSafeString(arguments[0]);
                writer.WriteSafeString(" ");
                writer.WriteSafeString(arguments[1]);
            });
            
            handlebars.RegisterHelper(getData, (context, arguments) => arguments[0]);
            
            var result = template(new object());
            Assert.Equal("data 42", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void BasicLookup(IHandlebars handlebars)
        {
            var source = "{{#each people}}{{.}} lives in {{lookup ../cities @index}} {{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                people = new[]{"Nils", "Yehuda"},
                cities = new[]{"Darmstadt", "San Francisco"}
            };
            
            var result = template(data);
            Assert.Equal("Nils lives in Darmstadt Yehuda lives in San Francisco ", result);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        public void LookupAsSubExpression(IHandlebars handlebars)
        {
            var source = "{{#each persons}}{{name}} lives in {{#with (lookup ../cities [resident])~}}{{name}} ({{country}}){{/with}}{{/each}}";
            var template = handlebars.Compile(source);
            var data = new
            {
                persons = new[]
                {
                    new
                    {
                        name = "Nils",
                        resident = "darmstadt"
                    },
                    new
                    {
                        name = "Yehuda",
                        resident = "san-francisco"
                    }
                },
                cities = new Dictionary<string, object>
                {
                    ["darmstadt"] = new
                    {
                        name = "Darmstadt",
                        country = "Germany"
                    },
                    ["san-francisco"] = new
                    {
                        name = "San Francisco",
                        country = "USA"
                    }
                }
            };
            
            var result = template(data);
            Assert.Equal("Nils lives in Darmstadt (Germany)Yehuda lives in San Francisco (USA)", result);
        }

        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        private void StringConditionTest(IHandlebars handlebars)
        {
            var expected = "\"correo\": \"correo@gmail.com\"";
            var template = "{{#if Email}}\"correo\": \"{{Email}}\"{{else}}\"correo\": \"no hay correo\",{{/if}}";
            var data = new
            {
                Email = "correo@gmail.com"
            };

            var func = handlebars.Compile(template);
            var actual = func(data);
            
            Assert.Equal(expected, actual);
        }
        
        [Theory, ClassData(typeof(HandlebarsEnvGenerator))]
        private void CustomHelperResolverTest(IHandlebars handlebars)
        {
            handlebars.Configuration.HelperResolvers.Add(new StringHelperResolver());
            var template = "{{ toLower input }}";
            var func = handlebars.Compile(template);
            var data = new { input = "ABC" };
            
            var actual = func(data);
            
            Assert.Equal(data.input.ToLower(), actual);
        }

        [Theory]
        [InlineData("[one].two")]
        [InlineData("one.[two]")]
        [InlineData("[one].[two]")]
        [InlineData("one.two")]
        public void ReferencingDirectlyVariableWhenHelperRegistered(string helperName)
        {
            var source = "{{ ./" + helperName + " }}";
            
            foreach (IHandlebars handlebars in new HandlebarsEnvGenerator().Select(o => o[0]))
            {
                handlebars.RegisterHelper("one.two", (context, arguments) => 0);

                var template = handlebars.Compile(source);

                var actual = template(new { one = new { two = 42 } });
            
                Assert.Equal("42", actual);   
            }
        }
        
        private class StringHelperResolver : IHelperResolver
        {
            public bool TryResolveHelper(PathInfo name, Type targetType, out IHelperDescriptor<HelperOptions> helper)
            {
                if (targetType == typeof(string))
                {
                    var method = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                        .FirstOrDefault(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase));

                    if (method == null)
                    {
                        helper = null;
                        return false;
                    }
                    
                    helper = new HelperDescriptor(name, method);
                    return true;
                }
                
                helper = null;
                return false;
            }

            public bool TryResolveBlockHelper(PathInfo name, out IHelperDescriptor<BlockHelperOptions> helper)
            {
                helper = null;
                return false;
            }
            
            private class HelperDescriptor : IHelperDescriptor<HelperOptions>
            {
                private readonly MethodInfo _methodInfo;

                public HelperDescriptor(PathInfo name, MethodInfo methodInfo)
                {
                    _methodInfo = methodInfo;
                    Name = name;
                }

                public PathInfo Name { get; }
                public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
                {
                    return _methodInfo.Invoke(arguments[0], arguments.AsEnumerable().Skip(1).ToArray());
                }

                public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
                {
                    output.Write(Invoke(options, context, arguments));
                }
            }
        }
        
        private class CustomUndefinedFormatter : IFormatter, IFormatterProvider
        {
            public void Format<T>(T value, in EncodedTextWriter writer)
            {
                writer.Write($"('{(value as UndefinedBindingResult)!.Value}' is undefined)");
            }

            public bool TryCreateFormatter(Type type, out IFormatter formatter)
            {
                if (type != typeof(UndefinedBindingResult))
                {
                    formatter = null;
                    return false;
                }

                formatter = this;
                return true;
            }
        }
        
        private class CustomDateTimeFormatter : IFormatter, IFormatterProvider
        {
            private readonly string _format;

            public CustomDateTimeFormatter(string format) => _format = format;

            public void Format<T>(T value, in EncodedTextWriter writer)
            {
                if(!(value is DateTime dateTime)) 
                    throw new ArgumentException("supposed to be DateTime");
                
                writer.Write($"{dateTime.ToString(_format)}");
            }

            public bool TryCreateFormatter(Type type, out IFormatter formatter)
            {
                if (type != typeof(DateTime))
                {
                    formatter = null;
                    return false;
                }

                formatter = this;
                return true;
            }
        }

        private class MockDictionary : IDictionary<string, string>
        {
            public void Add(string key, string value)
            {
                throw new NotImplementedException();
            }
            public bool ContainsKey(string key)
            {
                throw new NotImplementedException();
            }
            public bool Remove(string key)
            {
                throw new NotImplementedException();
            }
            public bool TryGetValue(string key, out string value)
            {
                value = "Hello world!";
                return true;
            }
            public string this[string index]
            {
                get
                {
                    return "Hello world!";
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
            public ICollection<string> Keys
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            public ICollection<string> Values
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            public void Add(KeyValuePair<string, string> item)
            {
                throw new NotImplementedException();
            }
            public void Clear()
            {
                throw new NotImplementedException();
            }
            public bool Contains(KeyValuePair<string, string> item)
            {
                throw new NotImplementedException();
            }
            public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }
            public bool Remove(KeyValuePair<string, string> item)
            {
                throw new NotImplementedException();
            }
            public int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            public bool IsReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                throw new NotImplementedException();
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }


        public class MockDictionaryImplicitlyImplemented : IDictionary<string, object>
        {
            private Dictionary<string, object> data;

            /// <inheritdoc />
            public MockDictionaryImplicitlyImplemented(Dictionary<string, object> data)
            {
                this.data = data;
            }

            bool IDictionary<string, object>.ContainsKey(string key)
            {
                return ((IDictionary<string, object>)data).ContainsKey(key);
            }

            public object this[string key] { get => ((IDictionary<string, object>)data)[key]; set => ((IDictionary<string, object>)data)[key] = value; }
            public ICollection<string> Keys => ((IDictionary<string, object>)data).Keys;
            public ICollection<object> Values => ((IDictionary<string, object>)data).Values;
            public int Count => ((IDictionary<string, object>)data).Count;
            public bool IsReadOnly => ((IDictionary<string, object>)data).IsReadOnly;

            public void Add(string key, object value)
            {
                ((IDictionary<string, object>)data).Add(key, value);
            }

            public void Add(KeyValuePair<string, object> item)
            {
                ((IDictionary<string, object>)data).Add(item);
            }

            public void Clear()
            {
                ((IDictionary<string, object>)data).Clear();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                return ((IDictionary<string, object>)data).Contains(item);
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                ((IDictionary<string, object>)data).CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return ((IDictionary<string, object>)data).GetEnumerator();
            }

            public bool Remove(string key)
            {
                return ((IDictionary<string, object>)data).Remove(key);
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                return ((IDictionary<string, object>)data).Remove(item);
            }

            public bool TryGetValue(string key, out object value)
            {
                return ((IDictionary<string, object>)data).TryGetValue(key, out value);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((IDictionary<string, object>)data).GetEnumerator();
            }
        }
    }
}

