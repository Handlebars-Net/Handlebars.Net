using Xunit;
using System;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace HandlebarsDotNet.Test
{
    public class DynamicTests
    {
        [Fact]
		public void DynamicObjectBasicTest()
        {
            var model = new MyDynamicModel();

            var source = "Foo: {{foo}}\nBar: {{bar}}";

            var template = Handlebars.Compile(source);

            var output = template(model);

            Assert.Equal("Foo: 1\nBar: hello world", output);
        }

		[Fact]
		public void JsonTestIfTruthy()
		{
			var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>("{\"myfield\":\"test1\",\"truthy\":\"test2\"}");

			var source = "{{myfield}}{{#if truthy}}{{truthy}}{{/if}}";

			var template = Handlebars.Compile(source);

			var output = template(model);

			Assert.Equal("test1test2", output);
		}

		[Fact]
		public void JsonTestIfFalsyMissingField()
		{
			var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>("{\"myfield\":\"test1\"}");

			var source = "{{myfield}}{{#if mymissingfield}}{{mymissingfield}}{{/if}}";

			var template = Handlebars.Compile(source);

			var output = template(model);

			Assert.Equal("test1", output);
		}

		[Fact]
		public void JsonTestIfFalsyValue()
		{
			var model = Newtonsoft.Json.JsonConvert.DeserializeObject<ExpandoObject>("{\"myfield\":\"test1\",\"falsy\":null}");

			var source = "{{myfield}}{{#if falsy}}{{falsy}}{{/if}}";

			var template = Handlebars.Compile(source);

			var output = template(model);

			Assert.Equal("test1", output);
		}

        [Fact]
        public void JsonTestArrays(){
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>("[{\"Key\": \"Key1\", \"Value\": \"Val1\"},{\"Key\": \"Key2\", \"Value\": \"Val2\"}]");

            var source = "{{#each this}}{{Key}}{{Value}}{{/each}}";

            var template = Handlebars.Compile(source);

            var output = template(model);

            Assert.Equal("Key1Val1Key2Val2", output);
        }
        
        [Fact]
        public void JsonTestObjects(){
	        var model = JObject.Parse("{\"Key1\": \"Val1\", \"Key2\": \"Val2\"}");

	        var source = "{{#each this}}{{@key}}{{@value}}{{/each}}";

	        var template = Handlebars.Compile(source);

	        var output = template(model);

	        Assert.Equal("Key1Val1Key2Val2", output);
        }

        [Fact]
        public void JObjectTest() {
            object nullValue = null;
            JObject model = JObject.FromObject(new { Nested = new { Prop = "Prop" }, Nested2 = nullValue });

            var source = "{{NotExists.Prop}}";

            var template = Handlebars.Compile(source);

            var output = template(model);

            Assert.Equal("", output);
        }

#if !netstandard

        [Fact]
        public void SystemJsonTestArrays()
        {
            var model = System.Web.Helpers.Json.Decode("[{\"Key\": \"Key1\", \"Value\": \"Val1\"},{\"Key\": \"Key2\", \"Value\": \"Val2\"}]");

            var source = "{{#each this}}{{Key}}{{Value}}{{/each}}";

            var template = Handlebars.Compile(source);

            var output = template(model);

            Assert.Equal("Key1Val1Key2Val2", output);
        }

#endif

        private class MyDynamicModel : DynamicObject
        {
            private Dictionary<string, object> properties = new Dictionary<string, object>
            {
                { "foo", 1 },
                { "bar", "hello world" }
            };

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if(properties.ContainsKey(binder.Name))
                {
                    result = properties[binder.Name];
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
        }
    }
}

