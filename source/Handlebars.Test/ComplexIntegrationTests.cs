using NUnit.Framework;
using System;
using System.IO;
using System.Collections;

namespace HandlebarsDotNet.Test
{
    [TestFixture]
    public class ComplexIntegrationTests
    {
		private const string naturalLanguageListTemplate = "{{#each County}}" +
			"{{#if @first}}" + 
			"{{this}}" +
			"{{else}}" +
			"{{#if @last}}" +
			" and {{this}}" +
			"{{else}}" +
			", {{this}}" +
			"{{/if}}" +
			"{{/if}}" +
			"{{/each}}";

        [Test]
        public void DeepIf()
        {
            var source = 
@"{{#if outer_bool}}
{{#with a}}{{#if inner_bool}}a is true{{else}}a is false{{/if}}{{/with}}
{{else}}
{{#with b}}{{#if inner_bool}}b is true{{else}}b is false{{/if}}{{/with}}
{{/if}}";
            var template = Handlebars.Compile(source);
            var trueTrue = new {
                outer_bool = true,
                a = new {
                    inner_bool = true
                }
            };
            var trueFalse = new {
                outer_bool = true,
                a = new {
                    inner_bool = false
                }
            };
            var falseTrue = new {
                outer_bool = false,
                b = new {
                    inner_bool = true
                }
            };
            var falseFalse = new {
                outer_bool = false,
                b = new {
                    inner_bool = false
                }
            };
            var resultTrueTrue = template(trueTrue);
            var resultTrueFalse = template(trueFalse);
            var resultFalseTrue = template(falseTrue);
            var resultFalseFalse = template(falseFalse);
            Assert.AreEqual(Environment.NewLine + "a is true" + Environment.NewLine, resultTrueTrue);
            Assert.AreEqual(Environment.NewLine + "a is false" + Environment.NewLine, resultTrueFalse);
            Assert.AreEqual(Environment.NewLine + "b is true" + Environment.NewLine, resultFalseTrue);
            Assert.AreEqual(Environment.NewLine + "b is false" + Environment.NewLine, resultFalseFalse);
        }

        [Test]
        public void IfImplicitIteratorHelper()
        {
            var source = "{{#if outer_bool}}{{#items}}{{link_to url text}}{{/items}}{{/if}}";

            var template = Handlebars.Compile(source);

            var data = new {
                outer_bool = true,
                items = new [] 
                {
                    new { text = "Google", url = "http://google.com/" },
                    new { text = "Yahoo!", url = "http://yahoo.com/" }
                }
            };

            Handlebars.RegisterHelper("link_to", (writer, context, parameters) => {
                writer.WriteSafeString("<a href='" + parameters[0] + "'>" + parameters[1] + "</a>");
            });

            var result = template(data);
            Assert.AreEqual("<a href='http://google.com/'>Google</a><a href='http://yahoo.com/'>Yahoo!</a>", result);
        }

        [Test]
        public void BlockHelperHelper()
        {
            var source = "{{#block_helper foo}}{{link_to url text}}{{/block_helper}}";

            var data = new {
                foo = new [] 
                {
                    new { text = "Google", url = "http://google.com/" },
                    new { text = "Yahoo!", url = "http://yahoo.com/" }
                }
            };

            Handlebars.RegisterHelper("link_to", (writer, context, parameters) => {
                writer.WriteSafeString("<a href='" + parameters[0] + "'>" + parameters[1] + "</a>");
            });

            Handlebars.RegisterHelper("block_helper", (writer, options, context, arguments) => {
                foreach(var item in arguments[0] as IEnumerable)
                {
                    options.Template(writer, item);
                }
            });

			var template = Handlebars.Compile(source);

            var result = template(data);
            Assert.AreEqual("<a href='http://google.com/'>Google</a><a href='http://yahoo.com/'>Yahoo!</a>", result);
        }

        [Test]
        public void ContextTest()
        {
            var template = Handlebars.Compile(@"{{#each Foo}}{{../Bar}}: {{#each this}}{{../../Bar}}{{this}},{{/each}};{{/each}}");

            var expected = @"Foo: FooAA,FooAAA,;Foo: FooBB,FooBBB,;";

            var result = template(new
            {
                Bar = "Foo",
                Foo = new[] { 
                    new [] {
                        "AA",
                        "AAA"
                    },
                    new [] {
                        "BB",
                        "BBB"
                    }
                }
            });

            Assert.AreEqual(expected, result);
        }
			
		[Test]
		public void CountyHasOneValue()
		{
			var data = new
			{
				County = new[] { "Kane" }
			};

			var template = Handlebars.Compile(naturalLanguageListTemplate);

			var result = template(data);

			Assert.That(result, Is.EqualTo("Kane"));
		}

		[Test]
		public void CountyHasTwoValue()
		{
			var data = new
			{
				County = new[] { "Kane", "Salt Lake" }
			};

			var template = Handlebars.Compile(naturalLanguageListTemplate);

			var result = template(data);

			Assert.That(result, Is.EqualTo("Kane and Salt Lake"));
		}

		[Test]
		public void CountyHasMoreThanTwoValue()
		{
			var data = new
			{
				County = new[] { "Kane", "Salt Lake", "Weber" }
			};

			var template = Handlebars.Compile(naturalLanguageListTemplate);

			var result = template(data);

			Assert.That(result, Is.EqualTo("Kane, Salt Lake and Weber"));
		}

        [Test]
        public void PartialWithRoot()
        {
            string source = "{{>personcity}}!";

            var template = Handlebars.Compile(source);

            var data = new {
                name = "Marc",
                city = "Wilmington"
            };

            var partialSource = "{{name}} is from {{@root.city}}";
            using(var reader = new StringReader(partialSource))
            {
                var partialTemplate = Handlebars.Compile(reader);
                Handlebars.RegisterTemplate("personcity", partialTemplate);
            }

            var result = template(data);
            Assert.AreEqual("Marc is from Wilmington!", result);
        }
    }
}

