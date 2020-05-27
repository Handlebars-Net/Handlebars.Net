using Xunit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using HandlebarsDotNet.Compiler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace HandlebarsDotNet.Test
{
	[DataContract]
	public class TestJSONObject
	{
		[DataMember]
		public virtual string Description { get; set; }

		[DataMember]
		public virtual string Name { get; set; }


		[DataMember]
		public virtual TestJSONObject Test{ get; set; }
	}
	public class JsonIntegrationTests
	{
		[Fact]
		public void BasicJSON()
		{
			var json = new TestJSONObject()
			{
				Name = "Thing - {{Thing.Name}}",
				Description = "A description of {{Thing.Name}}",
				Test = new TestJSONObject()
				{
					Test = new TestJSONObject()
					{
						Name = "{{Thing.Name}}"
					}
				}
			};

			var source = JsonConvert.SerializeObject(json);

			var template = Handlebars.Compile(source);

			var data = new
			{
				Thing = new
				{
					Name = "Handlebars.Net"
				}
			};

			var result = template(data);

			Assert.NotNull(result);

			var resultJson = JsonConvert.DeserializeObject<TestJSONObject>(result);

			Assert.Equal($"Thing - {data.Thing.Name}", resultJson.Name);
			Assert.Equal($"A description of {data.Thing.Name}", resultJson.Description);
			Assert.Equal(data.Thing.Name, resultJson.Test.Test.Name);
		}

		[Fact]
		public void BasicWwithEscapeJSON()
		{
			var json = new TestJSONObject()
			{
				Name = "Thing - {{Thing.Name}}",
				Description = "**\\*.{{Thing.Name}}",
				Test = new TestJSONObject()
				{
					Test = new TestJSONObject()
					{
						Name = "\"{{Thing.Name}}\""
					}
				}
			};

			var source = JsonConvert.SerializeObject(json);

			var template = Handlebars.Compile(source);

			var data = new
			{
				Thing = new
				{
					Name = "Handlebars.Net"
				}
			};

			var result = template(data);

			Assert.NotNull(result);

			var resultJson = JsonConvert.DeserializeObject<TestJSONObject>(result);

			Assert.Equal($"Thing - {data.Thing.Name}", resultJson.Name);
			Assert.Equal($@"**\*.{data.Thing.Name}", resultJson.Description);
			Assert.Equal($@"""{data.Thing.Name}""", resultJson.Test.Test.Name);
		}
	}
}

