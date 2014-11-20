using NUnit.Framework;
using System;
using System.IO;

namespace Handlebars.Test
{
    [TestFixture]
    public class BasicIntegrationTests
    {
        [Test]
        public void BasicPath()
        {
            var source = "Hello, {{name}}!";
            var template = Handlebars.Compile(source);
            var data = new {
                name = "Handlebars.Net"
            };
            var result = template(data);
            Assert.AreEqual("Hello, Handlebars.Net!", result);
        }

        [Test]
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
            Assert.AreEqual("Hello, Bob!", resultTrue);
            Assert.AreEqual("Hello, Sam!", resultFalse);
        }

        [Test]
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
            Assert.AreEqual("Hello, my good friend Erik!", result);
        }

        [Test]
        public void BasicIterator()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{/each}}";
            var template = Handlebars.Compile(source);
            var data = new {
                people = new []{
                    new { 
                        name = "Erik"
                    },
                    new {
                        name = "Helen"
                    }
                }
            };
            var result = template(data);
            Assert.AreEqual("Hello,\n- Erik\n- Helen", result);
        }

        [Test]
        public void BasicIteratorWithIndex()
        {
            var source = "Hello,{{#each people}}\n{{@index}}. {{name}}{{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
            {
                people = new[]{
                    new { 
                        name = "Erik"
                    },
                    new {
                        name = "Helen"
                    }
                }
            };
            var result = template(data);
            Assert.AreEqual("Hello,\n0. Erik\n1. Helen", result);
        }

        [Test]
        public void BasicIteratorWithFirst()
        {
			var source = "Hello,{{#each people}}\n{{@index}}. {{name}} ({{name}} is {{#if @first}}first{{else}}not first{{/if}}){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
            {
                people = new[]{
                    new { 
                        name = "Erik"
                    },
                    new {
                        name = "Helen"
                    }
                }
            };
            var result = template(data);
            Assert.AreEqual("Hello,\n0. Erik (Erik is first)\n1. Helen (Helen is not first)", result);
        }

        [Test]
        public void BasicIteratorWithLast()
        {
			var source = "Hello,{{#each people}}\n{{@index}}. {{name}} ({{name}} is {{#if @last}}last{{else}}not last{{/if}}){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
            {
                people = new[]{
                    new { 
                        name = "Erik"
                    },
                    new {
                        name = "Helen"
                    }
                }
            };
            var result = template(data);
            Assert.AreEqual("Hello,\n0. Erik (Erik is not last)\n1. Helen (Helen is last)", result);
        }

        [Test]
        public void BasicIteratorEmpty()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{else}} (no one listed){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
            {
                people = new object[] { }
            };
            var result = template(data);
            Assert.AreEqual("Hello, (no one listed)", result);
        }

        [Test]
        public void BasicEncoding()
        {
            var source = "Hello, {{name}}!";
            var template = Handlebars.Compile(source);
            var data = new
            {
                name = "<b>Bob</b>"
            };
            var result = template(data);
            Assert.AreEqual("Hello, &lt;b&gt;Bob&lt;/b&gt;!", result);
        }

        [Test]
        public void BasicComment()
        {
            var source = "Hello, {{!don't render me!}}{{name}}!";
            var template = Handlebars.Compile(source);
            var data = new
            {
                name = "Carl"
            };
            var result = template(data);
            Assert.AreEqual("Hello, Carl!", result);
        }

        [Test]
        public void BasicCommentEscaped()
        {
            var source = "Hello, {{!--don't {{render}} me!--}}{{name}}!";
            var template = Handlebars.Compile(source);
            var data = new
            {
                name = "Carl"
            };
            var result = template(data);
            Assert.AreEqual("Hello, Carl!", result);
        }

        [Test]
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
            Assert.AreEqual("hello world ", result);
        }

        [Test]
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
            Assert.AreEqual("foo: hello bar: world ", result);
        }

        [Test]
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
            Assert.AreEqual("Click here: <a href='https://github.com/rexm/handlebars.net'>Handlebars.Net</a>", result);
        }

		[Test]
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


			Assert.AreEqual("Click here: <a href='https://github.com/rexm/handlebars.net'>Handlebars.Net</a>", result);
		}

        [Test]
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
            Assert.AreEqual("Hello, Bill!", result);
        }

        [Test]
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
            Assert.AreEqual("Hello, Bill Mary !", result);
        }

        [Test]
        public void BasicDeferredBlockNegated()
        {
            string source = "Hello, {{^people}}nobody{{/people}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                people = new string[] {
                }
            };

            var result = template(data);
            Assert.AreEqual("Hello, nobody!", result);
        }

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
		public void BasicPropertyMissing()
		{
			string source = "Hello, {{first}} {{last}}!";

			var template = Handlebars.Compile(source);

			var data = new {
				first = "Marc"
			};

			var result = template(data);
			Assert.AreEqual("Hello, Marc !", result);
		}

		[Test]
		public void BasicNumericFalsy()
		{
			string source = "Hello, {{#if falsy}}Truthy!{{/if}}";

			var template = Handlebars.Compile(source);

			var data = new {
				falsy = 0
			};

			var result = template(data);
			Assert.AreEqual("Hello, ", result);
		}

		[Test]
		public void BasicNumericTruthy()
		{
			string source = "Hello, {{#if truthy}}Truthy!{{/if}}";

			var template = Handlebars.Compile(source);

			var data = new {
				truthy = -0.1
			};

			var result = template(data);
			Assert.AreEqual("Hello, Truthy!", result);
		}

		[Test]
		public void BasicStringFalsy()
		{
			string source = "Hello, {{#if falsy}}Truthy!{{/if}}";

			var template = Handlebars.Compile(source);

			var data = new {
				falsy = ""
			};

			var result = template(data);
			Assert.AreEqual("Hello, ", result);
		}

		[Test]
		public void BasicTripleStash()
		{
			string source = "Hello, {{{dangerous_value}}}!";

			var template = Handlebars.Compile(source);

			var data = new {
				dangerous_value = "<div>There's HTML here</div>"
			};

			var result = template(data);
			Assert.AreEqual("Hello, <div>There's HTML here</div>!", result);
		}
    }
}

