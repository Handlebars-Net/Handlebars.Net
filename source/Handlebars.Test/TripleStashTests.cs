using System;
using NUnit.Framework;
using System.IO;

namespace HandlebarsDotNet.Test
{
	[TestFixture]
	public class TripleStashTests
	{
		[Test]
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
			Assert.AreEqual("Hello, <div>Marc</div>!", result);
		}

		[Test]
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
			Assert.AreEqual("Hello, <div><div>Marc</div></div>!", result);
		}

		[Test]
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
			Assert.AreEqual("<div>hello</div> <div>world</div> ", result);
		}

        [Test]
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
            Assert.AreEqual("Hello, <div>There's HTML here</div>!", result);
        }
	}
}

