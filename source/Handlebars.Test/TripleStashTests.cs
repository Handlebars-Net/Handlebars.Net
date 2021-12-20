using System;
using Xunit;
using System.IO;

namespace HandlebarsDotNet.Test
{
	public class TripleStashTests
	{
		[Fact]
		public void UnencodedPartial()
		{
			string source = "Hello, {{{>unenc_person}}}!";

			var template = Handlebars.Compile(source);

			var data = new {
				name = "Marc"
			};

			var partialSource = "<div>{{name}}</div>";
			using(var reader = new StringReader(partialSource))
			{
				var partialTemplate = Handlebars.Compile(reader);
				Handlebars.RegisterTemplate("unenc_person", partialTemplate);
			}

			var result = template(data);
			Assert.Equal("Hello, <div>Marc</div>!", result);
		}

		[Fact]
		public void EncodedPartialWithUnencodedContents()
		{
			string source = "Hello, {{>enc_person}}!";

			var template = Handlebars.Compile(source);

			var data = new {
				name = "<div>Marc</div>"
			};

			var partialSource = "<div>{{{name}}}</div>";
			using(var reader = new StringReader(partialSource))
			{
				var partialTemplate = Handlebars.Compile(reader);
				Handlebars.RegisterTemplate("enc_person", partialTemplate);
			}

			var result = template(data);
			Assert.Equal("Hello, <div><div>Marc</div></div>!", result);
		}

		[Fact]
		public void UnencodedObjectEnumeratorItems()
		{
			var source = "{{#each enumerateMe}}{{{this}}} {{/each}}";
			var template = Handlebars.Compile(source);
			var data = new
			{
				enumerateMe = new
				{
					foo = "<div>hello</div>",
					bar = "<div>world</div>"
				}
			};
			var result = template(data);
			Assert.Equal("<div>hello</div> <div>world</div> ", result);
		}

        [Fact]
        public void FailingBasicTripleStash()
        {
            string source = "{{#if a_bool}}{{{dangerous_value}}}{{/if}}Hello, {{{dangerous_value}}}!";

            var template = Handlebars.Compile(source);

            var data = new
                {
                    a_bool = false,
                    dangerous_value = "<div>There's HTML here</div>"
                };

            var result = template(data);
            Assert.Equal("Hello, <div>There's HTML here</div>!", result);
        }

		[Fact]
        public void UnencodedEncodedUnencoded()
        {
            string source = "{{{dangerous_value}}}...{{dangerous_value}}...{{{dangerous_value}}}!";

            var template = Handlebars.Compile(source);

            var data = new
                {
                    a_bool = false,
                    dangerous_value = "<div>There is HTML here</div>"
                };

            var result = template(data);
            Assert.Equal("<div>There is HTML here</div>...&lt;div&gt;There is HTML here&lt;/div&gt;...<div>There is HTML here</div>!", result);
        }
	}
}

