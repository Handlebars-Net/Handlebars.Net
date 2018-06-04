using Xunit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet.Test
{
    public class BasicIntegrationTests
    {
        [Fact]
        public void BasicPath()
        {
            var source = "Hello, {{name}}!";
            var template = Handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Fact]
        public void EmptyIf()
        {
            var source = 
@"{{#if false}}
{{else}}
{{/if}}";
            var template = Handlebars.Compile(source);
            var data = new
            {
            };
            var result = template(data);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void BasicPathUnresolvedBindingFormatter()
        {
            var source = "Hello, {{foo}}!";

	        var config = new HandlebarsConfiguration
	        {
		        UnresolvedBindingFormatter = "('{0}' is undefined)"
	        };
	        var handlebars = Handlebars.Create( config );

            var template = handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, ('foo' is undefined)!", result);
        }

		[Fact]
        public void BasicPathThrowOnUnresolvedBindingExpression()
        {
            var source = "Hello, {{foo}}!";

            var config = new HandlebarsConfiguration
	        {
		        ThrowOnUnresolvedBindingExpression = true
	        };
	        var handlebars = Handlebars.Create( config );
	        var template = handlebars.Compile( source );

            var data = new {
                name = "Handlebars.Net"
            };
	        Assert.Throws<HandlebarsUndefinedBindingException>( () => template( data ) );
        }

        [Fact]
        public void BasicPathThrowOnNestedUnresolvedBindingExpression()
        {
            var source = "Hello, {{foo.bar}}!";

            var config = new HandlebarsConfiguration
            {
                ThrowOnUnresolvedBindingExpression = true
            };
            var handlebars = Handlebars.Create(config);
            var template = handlebars.Compile(source);

            var data = new
            {
                foo = (object)null
            };
            var ex = Assert.Throws<HandlebarsUndefinedBindingException>(() => template(data));
            Assert.Equal("bar is undefined", ex.Message);
        }

        [Fact]
        public void BasicPathNoThrowOnNullExpression()
        {
            var source =
@"{{#if foo}}
{{foo.bar}}
{{else}}
false
{{/if}}
";

            var config = new HandlebarsConfiguration
            {
                ThrowOnUnresolvedBindingExpression = true
            };
            var handlebars = Handlebars.Create(config);
            var template = handlebars.Compile(source);

            var data = new
            {
                foo = (string)null
            };
            var result = template(data);
            Assert.Contains("false", result);
        }

        [Fact]
        public void AssertHandlebarsUndefinedBindingException()
        {
            var source = "Hello, {{person.firstname}} {{person.lastname}}!";

            var config = new HandlebarsConfiguration
            {
                ThrowOnUnresolvedBindingExpression = true
            };
            var handlebars = Handlebars.Create(config);
            var template = handlebars.Compile(source);

            var data = new
            {
                person = new
                {
                    firstname = "Erik"
                }
            };

            try
            {
                template(data);
            }
            catch (HandlebarsUndefinedBindingException ex)
            {
                Assert.Equal("person.lastname", ex.Path);
                Assert.Equal("lastname", ex.MissingKey);
                return;
            }

            Assert.False(true, "Exception is expected.");
        }

        [Fact]
        public void BasicPathWhiteSpace()
        {
            var source = "Hello, {{ name }}!";
            var template = Handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Fact]
        public void BasicCurlies()
        {
            var source = "Hello, {name}!";
            var template = Handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, {name}!", result);
        }

        [Fact]
        public void BasicCurliesWithLeadingSlash()
        {
            var source = "Hello, \\{name\\}!";
            var template = Handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal("Hello, \\{name\\}!", result);
        }

        [Fact]
        public void BasicCurliesWithEscapedLeadingSlash()
        {
            var source = @"Hello, \\{{name}}!";
            var template = Handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.Equal(@"Hello, \Handlebars.Net!", result);
        }

        [Fact]
        public void BasicPathArray()
        {
            var source = "Hello, {{ names.[1] }}!";
            var template = Handlebars.Compile(source);
            var data = new
            {
                names = new[] {"Foo", "Handlebars.Net"}
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Fact]
        public void BasicPathArrayChildPath()
        {
            var source = "Hello, {{ names.[1].name }}!";
            var template = Handlebars.Compile(source);
            var data = new
            {
                names = new[] {new {name = "Foo"}, new {name = "Handlebars.Net"}}
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }

        [Fact]
        public void BasicPathArrayNoSquareBracketsChildPath()
        {
            var source = "Hello, {{ names.1.name }}!";
            var template = Handlebars.Compile(source);
            var data = new
            {
                names = new[] { new { name = "Foo" }, new { name = "Handlebars.Net" } }
            };
            var result = template(data);
            Assert.Equal("Hello, Handlebars.Net!", result);
        }
        
        [Fact]
        public void BasicPathDotBinding()
        {
            var source = "{{#nestedObject}}{{.}}{{/nestedObject}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    nestedObject = "A dot goes a long way"
                };
            var result = template(data);
            Assert.Equal("A dot goes a long way", result);
        }

        [Fact]
        public void BasicPathRelativeDotBinding()
        {
            var source = "{{#nestedObject}}{{../.}}{{/nestedObject}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    nestedObject = "Relative dots, yay"
                };
            var result = template(data);
            Assert.Equal("{ nestedObject = Relative dots, yay }", result);
        }
        
        [Fact]
        public void BasicPropertyOnArray()
        {
            var source = "Array is {{ names.Length }} item(s) long";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    names = new[] { new { name = "Foo" }, new { name = "Handlebars.Net" } }
                };
            var result = template(data);
            Assert.Equal("Array is 2 item(s) long", result);
        }

        [Fact]
        public void BasicIfElse()
        {
            var source = "Hello, {{#if basic_bool}}Bob{{else}}Sam{{/if}}!";
            var template = Handlebars.Compile(source);
            var trueData = new {
                basic_bool = true
            };
            var falseData = new {
                basic_bool = false
            };
            var resultTrue = template(trueData);
            var resultFalse = template(falseData);
            Assert.Equal("Hello, Bob!", resultTrue);
            Assert.Equal("Hello, Sam!", resultFalse);
        }

        [Fact]
        public void BasicIfElseIf()
        {
            var source = "{{#if isActive}}active{{else if isInactive}}inactive{{/if}}";
            var template = Handlebars.Compile(source);
            var activeData = new {
                isActive = true
            };
            var inactiveData = new {
                isInactive = true
            };
            var resultTrue = template(activeData);
            var resultFalse = template(inactiveData);
            Assert.Equal("active", resultTrue);
            Assert.Equal("inactive", resultFalse);
        }

        [Fact]
        public void BasicIfElseIfElse()
        {
            var source = "{{#if isActive}}active{{else if isInactive}}inactive{{else}}nada{{/if}}";
            var template = Handlebars.Compile(source);
            var activeData = new {
                isActive = true
            };
            var inactiveData = new {
                isInactive = true
            };
            var elseData = new {
            };
            var resultActive = template(activeData);
            var resultInactive = template(inactiveData);
            var resultElse = template(elseData);
            Assert.Equal("active", resultActive);
            Assert.Equal("inactive", resultInactive);
            Assert.Equal("nada", resultElse);
        }

        [Fact]
        public void BasicWith()
        {
            var source = "Hello,{{#with person}} my good friend {{name}}{{/with}}!";
            var template = Handlebars.Compile(source);
            var data = new {
                person = new {
                    name = "Erik"
                }
            };
            var result = template(data);
            Assert.Equal("Hello, my good friend Erik!", result);
        }

        [Fact]
        public void BasicWithInversion()
        {
            var source = "Hello, {{#with person}} my good friend{{else}}nevermind{{/with}}";
            var template = Handlebars.Compile(source);

			Assert.Equal("Hello, nevermind", template(new {}));
			Assert.Equal("Hello, nevermind", template(new {person = false}));
			Assert.Equal("Hello, nevermind", template(new {person = new string[] {}}));
        }

        [Fact]
        public void BasicEncoding()
        {
            var source = "Hello, {{name}}!";
            var template = Handlebars.Compile(source);
            var data = new
            {
                name = "<b>Bob</b>"
            };
            var result = template(data);
            Assert.Equal("Hello, &lt;b&gt;Bob&lt;/b&gt;!", result);
        }

        [Fact]
        public void BasicComment()
        {
            var source = "Hello, {{!don't render me!}}{{name}}!";
            var template = Handlebars.Compile(source);
            var data = new
            {
                name = "Carl"
            };
            var result = template(data);
            Assert.Equal("Hello, Carl!", result);
        }

        [Fact]
        public void BasicCommentEscaped()
        {
            var source = "Hello, {{!--don't {{render}} me!--}}{{name}}!";
            var template = Handlebars.Compile(source);
            var data = new
            {
                name = "Carl"
            };
            var result = template(data);
            Assert.Equal("Hello, Carl!", result);
        }

        [Fact]
        public void BasicObjectEnumerator()
        {
            var source = "{{#each enumerateMe}}{{this}} {{/each}}";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicObjectEnumeratorWithKey()
        {
            var source = "{{#each enumerateMe}}{{@key}}: {{this}} {{/each}}";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicDictionaryEnumerator()
        {
            var source = "{{#each enumerateMe}}{{this}} {{/each}}";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicDictionaryEnumeratorWithIntKeys()
        {
            var source = "{{#each enumerateMe}}{{this}} {{/each}}";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicDictionaryEnumeratorWithKey()
        {
            var source = "{{#each enumerateMe}}{{@key}}: {{this}} {{/each}}";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicDictionaryEnumeratorWithLongKey()
        {
            var source = "{{#each enumerateMe}}{{@key}}: {{this}} {{/each}}";
            var template = Handlebars.Compile(source);
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


        [Fact]
        public void BasicPathDictionaryStringKeyNoSquareBrackets()
        {
            var source = "Hello, {{ names.Foo }}!";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicPathDictionaryStringKey()
        {
            var source = "Hello, {{ names.[Foo] }}!";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicPathDictionaryIntKeyNoSquareBrackets()
        {
            var source = "Hello, {{ names.42 }}!";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicPathDictionaryLongKeyNoSquareBrackets()
        {
            var source = "Hello, {{ names.42 }}!";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicPathDictionaryIntKey()
        {
            var source = "Hello, {{ names.[42] }}!";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void BasicPathDictionaryLongKey()
        {
            var source = "Hello, {{ names.[42] }}!";
            var template = Handlebars.Compile(source);
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


        [Fact]
        public void DynamicWithMetadataEnumerator()
        {
            var source = "{{#each enumerateMe}}{{this}} {{/each}}";
            var template = Handlebars.Compile(source);
            dynamic data = new ExpandoObject();
            data.enumerateMe = new ExpandoObject();
            data.enumerateMe.foo = "hello";
            data.enumerateMe.bar = "world";
            var result = template(data);
            Assert.Equal("hello world ", result);
        }

        [Fact]
        public void DynamicWithMetadataEnumeratorWithKey()
        {
            var source = "{{#each enumerateMe}}{{@key}}: {{this}} {{/each}}";
            var template = Handlebars.Compile(source);
            dynamic data = new ExpandoObject();
            data.enumerateMe = new ExpandoObject();
            data.enumerateMe.foo = "hello";
            data.enumerateMe.bar = "world";
            var result = template(data);
            Assert.Equal("foo: hello bar: world ", result);
        }

        [Fact]
        public void BasicHelper()
        {
            Handlebars.RegisterHelper("link_to", (writer, context, parameters) => {
                writer.WriteSafeString("<a href='" + parameters[0] + "'>" + parameters[1] + "</a>");
            });

            string source = @"Click here: {{link_to url text}}";

            var template = Handlebars.Compile(source);

            var data = new {
                url = "https://github.com/rexm/handlebars.net",
                text = "Handlebars.Net"
            };

            var result = template(data);
            Assert.Equal("Click here: <a href='https://github.com/rexm/handlebars.net'>Handlebars.Net</a>", result);
        }

		[Fact]
		public void BasicHelperPostRegister()
		{
			string source = @"Click here: {{link_to_post_reg url text}}";

			var template = Handlebars.Compile(source);

			Handlebars.RegisterHelper("link_to_post_reg", (writer, context, parameters) => {
				writer.WriteSafeString("<a href='" + parameters[0] + "'>" + parameters[1] + "</a>");
			});

			var data = new {
				url = "https://github.com/rexm/handlebars.net",
				text = "Handlebars.Net"
			};

			var result = template(data);


			Assert.Equal("Click here: <a href='https://github.com/rexm/handlebars.net'>Handlebars.Net</a>", result);
		}

        [Fact]
        public void BasicDeferredBlock()
        {
            string source = "Hello, {{#person}}{{name}}{{/person}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                person = new {
                    name = "Bill"
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Bill!", result);
        }

		[Fact]
        public void BasicDeferredBlockString()
        {
            string source = "{{#person}} -{{this}}- {{/person}}";
            
            var template = Handlebars.Compile(source);
            
            var result = template(new {person = "Bill"});
            Assert.Equal(" -Bill- ", result);
        }

        [Fact]
        public void BasicDeferredBlockWithWhitespace()
        {
            string source = "Hello, {{ # person }}{{ name }}{{ / person }}!";

            var template = Handlebars.Compile(source);

            var data = new {
                person = new {
                    name = "Bill"
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Bill!", result);
        }

        [Fact]
        public void BasicDeferredBlockFalsy()
        {
            string source = "Hello, {{#person}}{{name}}{{/person}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                person = false
            };

            var result = template(data);
            Assert.Equal("Hello, !", result);
        }

        [Fact]
        public void BasicDeferredBlockNull()
        {
            string source = "Hello, {{#person}}{{name}}{{/person}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                person = (object)null
            };

            var result = template(data);
            Assert.Equal("Hello, !", result);
        }

        [Fact]
        public void BasicDeferredBlockEnumerable()
        {
            string source = "Hello, {{#people}}{{this}} {{/people}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                people = new [] {
                    "Bill",
                    "Mary"
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Bill Mary !", result);
        }

        [Fact]
        public void BasicDeferredBlockNegated()
        {
            string source = "Hello, {{^people}}nobody{{/people}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                people = new string[] {
                }
            };

            var result = template(data);
            Assert.Equal("Hello, nobody!", result);
        }

        [Fact]
        public void BasicDeferredBlockNegatedContext()
        {
            var template = Handlebars.Compile("Hello, {{^obj}}{{name}}{{/obj}}!");
            
            Assert.Equal("Hello, nobody!", template(new {name = "nobody"}));
            Assert.Equal("Hello, nobody!", template(new {name = "nobody", obj = new string[0]}));
        }
        
        [Fact]
        public void BasicDeferredBlockInversion()
        {
            var template = Handlebars.Compile("Hello, {{#obj}}somebody{{else}}{{name}}{{/obj}}!");
        
            Assert.Equal("Hello, nobody!", template(new {name = "nobody"}));
            Assert.Equal("Hello, nobody!", template(new {name = "nobody", obj = false}));
            Assert.Equal("Hello, nobody!", template(new {name = "nobody", obj = new string[0]}));
        }
        
        [Fact]
        public void BasicDeferredBlockNegatedInversion()
        {
            var template = Handlebars.Compile("Hello, {{^obj}}nobody{{else}}{{name}}{{/obj}}!");
        
            var array = new[]
            {
                new {name = "John"},
                new {name = " and "},
                new {name = "Sarah"}
            };
        
            Assert.Equal("Hello, John and Sarah!", template(new {obj = array}));
            Assert.Equal("Hello, somebody!", template(new {obj = true, name = "somebody"}));
            Assert.Equal("Hello, person!", template(new {obj = new {name = "person"}}));
        }

		[Fact]
		public void BasicPropertyMissing()
		{
			string source = "Hello, {{first}} {{last}}!";

			var template = Handlebars.Compile(source);

			var data = new {
				first = "Marc"
			};

			var result = template(data);
			Assert.Equal("Hello, Marc !", result);
		}

        [Fact]
        public void BasicNullOrMissingSubProperty()
        {
            string source = "Hello, {{name.first}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                name = (object)null
            };

            var result = template(data);
            Assert.Equal("Hello, !", result);
        }

		[Fact]
		public void BasicNumericFalsy()
		{
			string source = "Hello, {{#if falsy}}Truthy!{{/if}}";

			var template = Handlebars.Compile(source);

			var data = new {
				falsy = 0
			};

			var result = template(data);
			Assert.Equal("Hello, ", result);
		}

        [Fact]
        public void BasicNullFalsy()
        {
            string source = "Hello, {{#if falsy}}Truthy!{{/if}}";

            var template = Handlebars.Compile(source);

            var data = new {
                falsy = (object)null
            };

            var result = template(data);
            Assert.Equal("Hello, ", result);
        }

		[Fact]
		public void BasicNumericTruthy()
		{
			string source = "Hello, {{#if truthy}}Truthy!{{/if}}";

			var template = Handlebars.Compile(source);

			var data = new {
				truthy = -0.1
			};

			var result = template(data);
			Assert.Equal("Hello, Truthy!", result);
		}

		[Fact]
		public void BasicStringFalsy()
		{
			string source = "Hello, {{#if falsy}}Truthy!{{/if}}";

			var template = Handlebars.Compile(source);

			var data = new {
				falsy = ""
			};

			var result = template(data);
			Assert.Equal("Hello, ", result);
		}

        [Fact]
        public void BasicEmptyArrayFalsy()
        {
            var source = "{{#if Array}}stuff: {{#each Array}}{{this}}{{/each}}{{/if}}";

            var template = Handlebars.Compile(source);

            var data = new {
                Array = new object[] {}
            };

            var result = template(data);

            Assert.Equal("", result);
        }

		[Fact]
		public void BasicTripleStash()
		{
			string source = "Hello, {{{dangerous_value}}}!";

			var template = Handlebars.Compile(source);

			var data = new {
				dangerous_value = "<div>There's HTML here</div>"
			};

			var result = template(data);
			Assert.Equal("Hello, <div>There's HTML here</div>!", result);
		}

        [Fact]
        public void BasicEscape()
        {
            string source = @"Hello, \{{raw_value}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                raw_value = "<div>I shouldn't display</div>"
            };

            var result = template(data);
            Assert.Equal(@"Hello, {{raw_value}}!", result);
        }

        [Fact]
        public void BasicNumberLiteral()
        {
            string source = "{{eval 2  3}}";

            Handlebars.RegisterHelper("eval",
                (writer, context, args) => writer.Write("{0} {1}", args[0], args[1]));

            var template = Handlebars.Compile(source);

            var data = new { };

            var result = template(data);
            Assert.Equal("2 3", result);
        }

        [Fact]
        public void BasicNullLiteral()
        {
            string source = "{{eval null}}";

            Handlebars.RegisterHelper("eval",
                (writer, context, args) => writer.Write(args[0] == null));

            var template = Handlebars.Compile(source);

            var data = new { };

            var result = template(data);
            Assert.Equal("True", result);
        }

        [Fact]
        public void BasicCurlyBracesInLiterals()
        {
            var source = @"{{verbatim '{{foo}}'}} something {{verbatim '{{bar}}'}}";

            Handlebars.RegisterHelper("verbatim",
                (writer, context, args) => writer.Write(args[0]));

            var template = Handlebars.Compile(source);
            
            var data = new { };
            var result = template(data);

            Assert.Equal("{{foo}} something {{bar}}", result);
        }
	    
        [Fact]
        public void BasicRoot()
        {
            string source = "{{#people}}- {{this}} is member of {{@root.group}}\n{{/people}}";

            var template = Handlebars.Compile(source);

            var data = new {
                group = "Engineering",
                people = new []
                    {
                        "Rex",
                        "Todd"
                    }
            };

            var result = template(data);
            Assert.Equal("- Rex is member of Engineering\n- Todd is member of Engineering\n", result);
        }

        [Fact]
        public void ImplicitConditionalBlock()
        {
            var template =
                "{{#home}}Welcome Home{{/home}}{{^home}}Welcome to {{newCity}}{{/home}}";

            var data = new {
                newCity = "New York City",
                oldCity = "Los Angeles",
                home = false
            };

            var compiler = Handlebars.Compile(template);
            var result = compiler.Invoke(data);
            Assert.Equal("Welcome to New York City", result);
        }

        [Fact]
        public void BasicDictionary()
        {
            var source =
                "<div id='userInfo'>UserName: {{userInfo.userName}} Language: {{userInfo.language}}</div>"
                + "<div id='main' style='width:{{clientSettings.width}}px; height:{{clientSettings.height}}px'>body</div>";

            var template = Handlebars.Compile(source);

            var embeded = new Dictionary<string, object>();
            embeded.Add("userInfo", 
                new
                {
                    userName = "Ondrej",
                    language = "Slovak"
                });
            embeded.Add("clientSettings",
                new
                {
                    width = 120,
                    height = 80
                });

            var result = template(embeded);
            var expectedResult = 
                "<div id='userInfo'>UserName: Ondrej Language: Slovak</div>"
                + "<div id='main' style='width:120px; height:80px'>body</div>";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void BasicHashtable()
        {
            var source = "{{dictionary.[key]}}";

            var template = Handlebars.Compile(source);

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

        [Fact]
        public void BasicHashtableNoSquareBrackets()
        {
            var source = "{{dictionary.key}}";

            var template = Handlebars.Compile(source);

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
        
        [Fact]
        public void BasicMockIDictionary()
        {
            var source = "{{dictionary.[key]}}";

            var template = Handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new MockDictionary()
            });
            var expectedResult = 
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void DictionaryWithSpaceInKeyName()
        {
            var source = "{{dictionary.[my key]}}";

            var template = Handlebars.Compile(source);

            var result = template(new
                {
                    dictionary = new MockDictionary()
                });
            var expectedResult = 
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void DictionaryWithSpaceInKeyNameAndChildProperty()
        {
            var source = "{{dictionary.[my key].prop1}}";

            var template = Handlebars.Compile(source);

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

        [Fact]
        public void BasicMockIDictionaryNoSquareBrackets()
        {
            var source = "{{dictionary.key}}";

            var template = Handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new MockDictionary()
            });
            var expectedResult =
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void BasicMockIDictionaryIntKey()
        {
            var source = "{{dictionary.[42]}}";

            var template = Handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new MockDictionary()
            });
            var expectedResult =
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void BasicMockIDictionaryIntKeyNoSquareBrackets()
        {
            var source = "{{dictionary.42}}";

            var template = Handlebars.Compile(source);

            var result = template(new
            {
                dictionary = new MockDictionary()
            });
            var expectedResult =
                "Hello world!";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void TestNoWhitespaceBetweenExpressions()
        {
            
            var source = @"{{#is ProgramID """"}}no program{{/is}}{{#is ProgramID ""1081""}}some program text{{/is}}";

            Handlebars.RegisterHelper("is", (output, options, context, args) =>
                {
                    if(args[0] == args[1])
                    {
                        options.Template(output, context);
                    }
                });


            var template = Handlebars.Compile(source);

            var result = template(new
                {
                    ProgramID = "1081"
                });
            
            var expectedResult =
                "some program text";

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void DictionaryIteration()
        {
            string source = @"{{#ADictionary}}{{@key}},{{value}}{{/ADictionary}}";
            var template = Handlebars.Compile(source);
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

        [Fact]
        public void ObjectEnumeration()
        {
            string source = @"{{#each myObject}}{{#if this.length}}<b>{{@key}}</b>{{#each this}}<li>{{this}}</li>{{/each}}<br>{{/if}}{{/each}}";
            var template = Handlebars.Compile(source);
            var result = template(new 
                {
                    myObject = new {
                        arr = new []{ "hello", "world" },
                        notArr = 1
                    }
                });

            Assert.Equal("<b>arr</b><li>hello</li><li>world</li><br>", result);
        }

        [Fact]
        public void NestedDictionaryWithSegmentLiteral()
        {
            var source = "{{dictionary.[my key].[another key]}}";

            var template = Handlebars.Compile(source);

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

        private class MockDictionary : IDictionary<string, string>
        {
            public void Add(string key, string value)
            {
                throw new NotImplementedException();
            }
            public bool ContainsKey(string key)
            {
                return true;
            }
            public bool Remove(string key)
            {
                throw new NotImplementedException();
            }
            public bool TryGetValue(string key, out string value)
            {
                throw new NotImplementedException();
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
    }
}

