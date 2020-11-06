using System;
using System.Collections;
using Xunit;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HandlebarsDotNet.Test
{
    public class DynamicTests
    {
        public class EnvGenerator : IEnumerable<object[]>
        {
            private readonly List<IHandlebars> _data = new List<IHandlebars>
            {
                Handlebars.Create()
            };

            public IEnumerator<object[]> GetEnumerator() => _data.Select(o => new object[] { o }).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        
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
        public void DynamicObjectBasicIterationTest()
        {
            var model = new MyDynamicModel();

            var source = "{{#each this}}{{@key}}: {{@value}}\n{{/each}}";

            var template = Handlebars.Compile(source);

            var output = template(model);

            Assert.Equal("foo: 1\nbar: hello world\n", output);
        }

		[Fact]
		public void JsonTestIfTruthy()
		{
			var model = JsonConvert.DeserializeObject<ExpandoObject>("{\"myfield\":\"test1\",\"truthy\":\"test2\"}");

			var source = "{{myfield}}{{#if truthy}}{{truthy}}{{/if}}";

			var template = Handlebars.Compile(source);

			var output = template(model);

			Assert.Equal("test1test2", output);
		}

		[Fact]
		public void JsonTestIfFalsyMissingField()
		{
			var model = JsonConvert.DeserializeObject<ExpandoObject>("{\"myfield\":\"test1\"}");

			var source = "{{myfield}}{{#if mymissingfield}}{{mymissingfield}}{{/if}}";

			var template = Handlebars.Compile(source);

			var output = template(model);

			Assert.Equal("test1", output);
		}

		[Fact]
		public void JsonTestIfFalsyValue()
		{
			var model = JsonConvert.DeserializeObject<ExpandoObject>("{\"myfield\":\"test1\",\"falsy\":null}");

			var source = "{{myfield}}{{#if falsy}}{{falsy}}{{/if}}";

			var template = Handlebars.Compile(source);

			var output = template(model);

			Assert.Equal("test1", output);
		}

        [Theory]
        [ClassData(typeof(EnvGenerator))]
        public void JsonTestArrays(IHandlebars handlebars){
            var model = JsonConvert.DeserializeObject("[{\"Key\": \"Key1\", \"Value\": \"Val1\"},{\"Key\": \"Key2\", \"Value\": \"Val2\"}]");

            var source = "{{#each this}}{{Key}}{{Value}}{{/each}}";

            var template = handlebars.Compile(source);

            var output = template(model);

            Assert.Equal("Key1Val1Key2Val2", output);
        }
        
        [Theory]
        [ClassData(typeof(EnvGenerator))]
        public void JsonTestArrayCount(IHandlebars handlebars)
        {
	        var model = JsonConvert.DeserializeObject("[{\"Key\": \"Key1\", \"Value\": \"Val1\"},{\"Key\": \"Key2\", \"Value\": \"Val2\"}]");

	        var source = "{{this.Count}}";

	        var template = handlebars.Compile(source);

	        var output = template(model);

	        Assert.Equal("2", output);
        }
        
        [Theory]
        [ClassData(typeof(EnvGenerator))]
        public void JsonTestObjects(IHandlebars handlebars){
	        var model = JObject.Parse("{\"Key1\": \"Val1\", \"Key2\": \"Val2\"}");

	        var source = "{{#each this}}{{@key}}{{@value}}{{/each}}";

	        var template = handlebars.Compile(source);

	        var output = template(model);

	        Assert.Equal("Key1Val1Key2Val2", output);
        }

        [Theory]
        [ClassData(typeof(EnvGenerator))]
        public void JObjectTest(IHandlebars handlebars) {
            object nullValue = null;
            JObject model = JObject.FromObject(new { Nested = new { Prop = "Prop" }, Nested2 = nullValue });

            var source = "{{NotExists.Prop}}";

            var template = handlebars.Compile(source);

            var output = template(model);

            Assert.Equal("", output);
        }
        
        [Theory]
        [ClassData(typeof(EnvGenerator))]
        public void WithParentIndexJsonNet(IHandlebars handlebars)
        {
            var source = @"
                {{#each level1}}
                    id={{id}}
                    index=[{{@../../index}}:{{@../index}}:{{@index}}]
                    first=[{{@../../first}}:{{@../first}}:{{@first}}]
                    last=[{{@../../last}}:{{@../last}}:{{@last}}]
                    {{#each level2}}
                        id={{id}}
                        index=[{{@../../index}}:{{@../index}}:{{@index}}]
                        first=[{{@../../first}}:{{@../first}}:{{@first}}]
                        last=[{{@../../last}}:{{@../last}}:{{@last}}]
                        {{#each level3}}
                            id={{id}}
                            index=[{{@../../index}}:{{@../index}}:{{@index}}]
                            first=[{{@../../first}}:{{@../first}}:{{@first}}]
                            last=[{{@../../last}}:{{@../last}}:{{@last}}]
                        {{/each}}
                    {{/each}}    
                {{/each}}";
            
            var template = handlebars.Compile( source );
            var data = new
                {
                    level1 = new[]{
                        new {
                            id = "0",
                            level2 = new[]{
                                new {
                                    id = "0-0",
                                    level3 = new[]{
                                        new { id = "0-0-0" },
                                        new { id = "0-0-1" }
                                    }
                                },
                                new {
                                    id = "0-1",
                                    level3 = new[]{
                                        new { id = "0-1-0" },
                                        new { id = "0-1-1" }
                                    }
                                }
                            }
                        },
                        new {
                            id = "1",
                            level2 = new[]{
                                new {
                                    id = "1-0",
                                    level3 = new[]{
                                        new { id = "1-0-0" },
                                        new { id = "1-0-1" }
                                    }
                                },
                                new {
                                    id = "1-1",
                                    level3 = new[]{
                                        new { id = "1-1-0" },
                                        new { id = "1-1-1" }
                                    }
                                }
                            }
                        }
                    }
            };

            var json = JObject.FromObject(data);

            var result = template(json);

            const string expected = @"
                            id=0
                            index=[::0]
                            first=[::True]
                            last=[::False]
                                id=0-0
                                index=[:0:0]
                                first=[:True:True]
                                last=[:False:False]
                                    id=0-0-0
                                    index=[0:0:0]
                                    first=[True:True:True]
                                    last=[False:False:False]
                                    id=0-0-1
                                    index=[0:0:1]
                                    first=[True:True:False]
                                    last=[False:False:True]
                                id=0-1
                                index=[:0:1]
                                first=[:True:False]
                                last=[:False:True]
                                    id=0-1-0
                                    index=[0:1:0]
                                    first=[True:False:True]
                                    last=[False:True:False]
                                    id=0-1-1
                                    index=[0:1:1]
                                    first=[True:False:False]
                                    last=[False:True:True]
                            id=1
                            index=[::1]
                            first=[::False]
                            last=[::True]
                                id=1-0
                                index=[:1:0]
                                first=[:False:True]
                                last=[:True:False]
                                    id=1-0-0
                                    index=[1:0:0]
                                    first=[False:True:True]
                                    last=[True:False:False]
                                    id=1-0-1
                                    index=[1:0:1]
                                    first=[False:True:False]
                                    last=[True:False:True]
                                id=1-1
                                index=[:1:1]
                                first=[:False:False]
                                last=[:True:True]
                                    id=1-1-0
                                    index=[1:1:0]
                                    first=[False:False:True]
                                    last=[True:True:False]
                                    id=1-1-1
                                    index=[1:1:1]
                                    first=[False:False:False]
                                    last=[True:True:True]";
            
            Func<string, string> makeFlat = text => text.Replace( " ", "" ).Replace( "\n", "" ).Replace( "\r", "" );

            Assert.Equal( makeFlat( expected ), makeFlat( result ) );
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
            private readonly Dictionary<string, object> _properties = new Dictionary<string, object>
            {
                { "foo", 1 },
                { "bar", "hello world" }
            };

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _properties.Keys;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if(_properties.ContainsKey(binder.Name))
                {
                    result = _properties[binder.Name];
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

