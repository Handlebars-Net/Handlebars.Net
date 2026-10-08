using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using HandlebarsDotNet.Features;
using HandlebarsDotNet.ObjectDescriptors;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public abstract class JsonTestBase
    {
        protected abstract object Parse(string json);

        [Fact]
        public void ResolvesNestedProperty()
        {
            var template = Handlebars.Create().Compile("{{A.B}}");

            Assert.Equal("b", template(Parse("{\"A\": {\"B\": \"b\"}}")));
        }

        [Fact]
        public void RendersScalarPropertyValues()
        {
            var template = Handlebars.Create().Compile("{{str}}|{{num}}|{{nothing}}");

            Assert.Equal("hello|42|", template(Parse("{\"str\": \"hello\", \"num\": 42, \"nothing\": null}")));
        }

        [Fact]
        public void IteratesArray()
        {
            var template = Handlebars.Create().Compile("{{#each items}}{{this}},{{/each}}");

            Assert.Equal("1,2,3,", template(Parse("{\"items\": [1, 2, 3]}")));
        }

        [Theory]
        [InlineData("{{#each this}}{{Name}},{{/each}}", "[{\"Name\": \"a\"}, {\"Name\": \"b\"}]")]
        [InlineData("{{#each items}}{{Name}},{{/each}}", "{\"items\": [{\"Name\": \"a\"}, {\"Name\": \"b\"}]}")]
        public void IteratesArrayOfObjects(string source, string json)
        {
            var template = Handlebars.Create().Compile(source);

            Assert.Equal("a,b,", template(Parse(json)));
        }

        [Fact]
        public void IteratesObjectProperties()
        {
            var template = Handlebars.Create().Compile("{{#each this}}{{@key}}={{this}} {{/each}}");

            Assert.Equal("A=1 B=2 ", template(Parse("{\"A\": \"1\", \"B\": \"2\"}")));
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("null", false)]
        [InlineData("\"\"", false)]
        [InlineData("\"text\"", true)]
        [InlineData("\"false\"", true)]
        [InlineData("0", false)]
        [InlineData("0.0", false)]
        [InlineData("1", true)]
        [InlineData("-1.5", true)]
        [InlineData("[]", false)]
        [InlineData("{}", false)]
        [InlineData("[null]", true)]
        [InlineData("{\"key\": null}", true)]
        public void RespectsConditionalTruthiness(string value, bool truthy)
        {
            var template = Handlebars.Create().Compile("{{#if value}}T{{else}}F{{/if}}|{{#unless value}}F{{else}}T{{/unless}}");

            Assert.Equal(truthy ? "T|T" : "F|F", template(Parse("{\"value\": " + value + "}")));
        }

        [Theory]
        [InlineData("true", "T")]
        [InlineData("false", "F")]
        [InlineData("null", "F")]
        [InlineData("0", "F")]
        [InlineData("\"\"", "F")]
        [InlineData("\"text\"", "T")]
        [InlineData("[]", "F")]
        [InlineData("{}", "F")]
        public void RespectsRootValueTruthiness(string json, string expected)
        {
            var template = Handlebars.Create().Compile("{{#if this}}T{{else}}F{{/if}}");

            Assert.Equal(expected, template(Parse(json)));
        }

        [Theory]
        [InlineData("0", "F|T")]
        [InlineData("0.0", "F|T")]
        [InlineData("1", "T|T")]
        public void HonorsIncludeZero(string value, string expected)
        {
            var template = Handlebars.Create().Compile("{{#if value includeZero=false}}T{{else}}F{{/if}}|{{#if value includeZero=true}}T{{else}}F{{/if}}");

            Assert.Equal(expected, template(Parse("{\"value\": " + value + "}")));
        }

        [Fact]
        public void ReturnsEmptyForMissingProperty()
        {
            var template = Handlebars.Create().Compile("{{Missing.Nested}}|{{#if Missing}}T{{else}}F{{/if}}");

            Assert.Equal("|F", template(Parse("{\"A\": \"a\"}")));
        }

        [Fact]
        public void ReturnsEmptyForNullAndScalarMemberAccess()
        {
            var template = Handlebars.Create().Compile("{{nullVal.Nested}}|{{intVal.Nested}}|{{stringVal.Nested}}");

            Assert.Equal("||", template(Parse("{\"nullVal\": null, \"intVal\": 1, \"stringVal\": \"a\"}")));
        }

        [Theory]
        [InlineData("{{Prop}}|{{prop}}", "{\"Prop\": \"a\", \"prop\": \"b\"}", "a|b")]
        [InlineData("{{PROP}}", "{\"prop\": \"a\"}", "a")]
        public void PrefersExactPropertyNameBeforeLowercaseFallback(string source, string json, string expected)
        {
            var template = Handlebars.Create().Compile(source);

            Assert.Equal(expected, template(Parse(json)));
        }

        [Theory]
        [InlineData("[]")]
        [InlineData("{}")]
        [InlineData("null")]
        [InlineData("42")]
        [InlineData("true")]
        [InlineData("\"text\"")]
        public void EachUsesInverseForEmptyOrScalarValues(string json)
        {
            var template = Handlebars.Create().Compile("{{#each this}}item{{else}}empty{{/each}}");

            Assert.Equal("empty", template(Parse(json)));
        }

        [Fact]
        public void IteratedValuesRespectTruthiness()
        {
            var template = Handlebars.Create().Compile("{{#each items}}{{#if this}}T{{else}}F{{/if}},{{/each}}");

            Assert.Equal("F,T,F,T,F,T,F,F,F,T,", template(Parse("{\"items\": [false, true, 0, 1, \"\", \"false\", null, [], {}, [1]]}")));
        }

        [Fact]
        public void PreservesArrayIterationMetadataAndBlockParameters()
        {
            var template = Handlebars.Create().Compile("{{#each items as |item index|}}{{@index}}:{{@key}}:{{@first}}:{{@last}}:{{index}}:{{item}}:{{@value}};{{/each}}");

            Assert.Equal("0:0:True:False:0:a:a;1:1:False:True:1:b:b;", template(Parse("{\"items\": [\"a\", \"b\"]}")));
        }

        [Fact]
        public void PreservesObjectIterationMetadataAndBlockParameters()
        {
            var template = Handlebars.Create().Compile("{{#each this as |value key|}}{{@index}}:{{@key}}:{{@first}}:{{@last}}:{{key}}:{{value}}:{{@value}};{{/each}}");

            Assert.Equal("0:A:True:False:A:a:a;1:B:False:True:B:b:b;", template(Parse("{\"A\": \"a\", \"B\": \"b\"}")));
        }

        [Fact]
        public void PreservesParentAndRootContextInNestedLoops()
        {
            var template = Handlebars.Create().Compile("{{#each groups as |group|}}{{#each items as |item index|}}{{@../index}}.{{index}}={{../stringVal}}/{{group.stringVal}}/{{item.stringVal}}/{{@root.stringVal}};{{/each}}{{/each}}");
            var data = Parse("{\"stringVal\": \"root\", \"groups\": [{\"stringVal\": \"a\", \"items\": [{\"stringVal\": \"x\"}, {\"stringVal\": \"y\"}]}, {\"stringVal\": \"b\", \"items\": [{\"stringVal\": \"z\"}]}]}");

            Assert.Equal("0.0=a/a/x/root;0.1=a/a/y/root;1.0=b/b/z/root;", template(data));
        }

        [Fact]
        public void WithPreservesParentAndRootContext()
        {
            var template = Handlebars.Create().Compile("{{#with objectVal}}{{stringVal}}:{{../rootVal}}:{{@root.rootVal}}{{/with}}");

            Assert.Equal("b:a:a", template(Parse("{\"rootVal\": \"a\", \"objectVal\": {\"stringVal\": \"b\"}}")));
        }

        [Theory]
        [InlineData("{{arrayVal.[0].stringVal}}|{{arrayVal.[1].stringVal}}", "a|b")]
        [InlineData("{{arrayVal.[2].stringVal}}|{{arrayVal.[-1].stringVal}}|{{arrayVal.[x].stringVal}}", "||")]
        [InlineData("{{scalars.[1]}}|{{lookup scalars 1}}|{{lookup scalars 5}}", "y|y|")]
        [InlineData("{{matrix.[1].[0]}}", "3")]
        [InlineData("{{#with arrayVal.[1]}}{{stringVal}}{{/with}}", "b")]
        [InlineData("{{#if scalars.[0]}}T{{else}}F{{/if}}{{#if scalars.[3]}}T{{else}}F{{/if}}", "TF")]
        public void ResolvesArrayIndexes(string source, string expected)
        {
            var template = Handlebars.Create().Compile(source);
            var data = Parse("{\"arrayVal\": [{\"stringVal\": \"a\"}, {\"stringVal\": \"b\"}], \"scalars\": [\"x\", \"y\"], \"matrix\": [[1, 2], [3, 4]]}");

            Assert.Equal(expected, template(data));
        }

        [Fact]
        public void PartialHashMergesContextProperties()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterTemplate("partial", "{{stringVal}}/{{extra}}");
            var template = handlebars.Compile("{{#with objectVal}}{{> partial extra='x'}}{{/with}}");

            Assert.Equal("b/x", template(Parse("{\"objectVal\": {\"stringVal\": \"b\"}}")));
        }

        [Fact]
        public void PreservesArrayIndexAndCountAccess()
        {
            var template = Handlebars.Create().Compile("{{arrayVal.[0].stringVal}}|{{arrayVal.[1].stringVal}}|{{arrayVal.Count}}|{{arrayVal.[2].stringVal}}|{{arrayVal.[-1].stringVal}}|{{emptyVal.Count}}");

            Assert.Equal("a|b|2|||0", template(Parse("{\"arrayVal\": [{\"stringVal\": \"a\"}, {\"stringVal\": \"b\"}], \"emptyVal\": []}")));
        }

        [Theory]
        [InlineData(false, "2|2||")]
        [InlineData(true, "2|2|2|2")]
        public void ArrayMembersUseConfiguredCollectionAliases(bool useAliases, string expected)
        {
            var handlebars = Handlebars.Create();
            if (useAliases)
            {
                handlebars.Configuration.UseCollectionMemberAliasProvider();
            }

            var template = handlebars.Compile("{{arrayVal.Count}}|{{arrayVal.count}}|{{arrayVal.Length}}|{{arrayVal.length}}");
            var data = Parse("{\"arrayVal\": [\"a\", \"b\"]}");

            Assert.Equal(expected, template(data));
            Assert.Equal(template(new { arrayVal = new List<string> { "a", "b" } }), template(data));
        }

        [Fact]
        public void ObjectMembersKeepJsonPropertyLookupWithCollectionAliases()
        {
            var handlebars = Handlebars.Create();
            handlebars.Configuration.UseCollectionMemberAliasProvider();
            var template = handlebars.Compile("{{objectVal.Count}}|{{objectVal.Length}}|{{objectVal.Options}}|{{emptyObj.Count}}|{{emptyObj.length}}|{{stringVal.Count}}|{{stringVal.length}}");

            Assert.Equal("7|8|||||", template(Parse("{\"objectVal\": {\"Count\": 7, \"Length\": 8}, \"emptyObj\": {}, \"stringVal\": \"abc\"}")));
        }
    }

    public class JsonElementTests : JsonTestBase
    {
        protected override object Parse(string json) => JsonSerializer.Deserialize<object>(json)!;

        [Fact]
        public void ArrayMembersUseCustomAliases()
        {
            var handlebars = Handlebars.Create();
            handlebars.Configuration.AliasProviders.Add(new DelegatedMemberAliasProvider().AddAlias<JsonElement>("itemCount", element => element.GetArrayLength()));
            var template = handlebars.Compile("{{arrayVal.itemCount}}|{{arrayVal.[0]}}");

            Assert.Equal("2|a", template(Parse("{\"arrayVal\": [\"a\", \"b\"]}")));
        }
    }

    public class JsonNodeTests : JsonTestBase
    {
        protected override object Parse(string json) => JsonNode.Parse(json)!;

        [Fact]
        public void CreatedNumericValuesRespectTruthiness()
        {
            var template = Handlebars.Create().Compile("{{#if value}}T{{else}}F{{/if}}|{{#if value includeZero=true}}T{{else}}F{{/if}}");
            var zeros = new JsonNode[]
            {
                JsonValue.Create(0)!, JsonValue.Create(0L)!, JsonValue.Create(0d)!, JsonValue.Create(0f)!,
                JsonValue.Create(0m)!, JsonValue.Create(0u)!, JsonValue.Create(0UL)!, JsonValue.Create((short) 0)!,
                JsonValue.Create((ushort) 0)!, JsonValue.Create((byte) 0)!, JsonValue.Create((sbyte) 0)!
            };

            foreach (var zero in zeros)
            {
                Assert.Equal("F|T", template(new JsonObject { ["value"] = zero }));
            }
        }

        [Fact]
        public void CreatedNodesRenderAndReflectChanges()
        {
            var data = new JsonObject
            {
                ["stringVal"] = "a",
                ["boolVal"] = false,
                ["arrayVal"] = new JsonArray(new JsonObject { ["intVal"] = 1 })
            };
            var template = Handlebars.Create().Compile("{{stringVal}}:{{#if boolVal}}T{{else}}F{{/if}}:{{#each arrayVal}}{{intVal}}{{/each}}");

            Assert.Equal("a:F:1", template(data));

            data["stringVal"] = "b";
            data["boolVal"] = true;
            data["arrayVal"]![0]!["intVal"] = 2;

            Assert.Equal("b:T:2", template(data));
        }

        [Fact]
        public void CreatedTextValuesRespectTruthiness()
        {
            var template = Handlebars.Create().Compile("{{#if this}}T{{else}}F{{/if}}");

            Assert.Equal("F", template(JsonValue.Create(string.Empty)));
            Assert.Equal("T", template(JsonValue.Create('a')));
            Assert.Equal("T", template(JsonValue.Create(Guid.Empty)));
            Assert.Equal("T", template(JsonValue.Create(DateTime.MinValue)));
        }

        [Fact]
        public void EachIteratesValuesWrappingClrContainers()
        {
            // These report JsonValueKind.Array/Object but are JsonValue, not JsonArray/JsonObject.
            var handlebars = Handlebars.Create();
            var nested = handlebars.Compile("{{#each items as |value key|}}{{key}}:{{value}},{{else}}empty{{/each}}");
            var root = handlebars.Compile("{{#each this as |value key|}}{{key}}:{{value}},{{else}}empty{{/each}}");
            var cases = new (JsonNode Value, string Expected)[]
            {
                (JsonValue.Create(new[] { 1, 2 })!, "0:1,1:2,"),
                (JsonValue.Create(new List<string> { "a", "b" })!, "0:a,1:b,"),
                (JsonValue.Create(new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 })!, "a:1,b:2,"),
                (JsonValue.Create(new WrappedItem())!, "Name:n,"),
                (JsonValue.Create(new AnnotatedItem())!, "Name:n,"),
                (JsonValue.Create(Array.Empty<int>())!, "empty"),
                (JsonValue.Create(new List<string>())!, "empty"),
                (JsonValue.Create(new Dictionary<string, int>())!, "empty")
            };

            foreach (var (value, expected) in cases)
            {
                Assert.Equal(expected, nested(new JsonObject { ["items"] = value }));
                Assert.Equal(expected, root(value));
            }
        }

        [Fact]
        public void MembersResolveOnValuesWrappingClrContainers()
        {
            // Members resolve against the wrapped CLR value, so CLR names apply rather than JSON property annotations.
            var handlebars = Handlebars.Create();
            var cases = new (JsonNode Value, string Source, string Expected)[]
            {
                (JsonValue.Create(new WrappedItem())!, "{{items.Name}}|{{#with items}}{{Name}}{{/with}}|{{lookup items 'Name'}}|{{items.Missing}}", "n|n|n|"),
                (JsonValue.Create(new List<string> { "a", "b" })!, "{{items.[0]}}|{{items.[1]}}|{{items.Count}}|{{lookup items 1}}|{{items.[2]}}", "a|b|2|b|"),
                (JsonValue.Create(new[] { 1, 2 })!, "{{items.[1]}}|{{items.Length}}", "2|2"),
                (JsonValue.Create(new Dictionary<string, int> { ["a"] = 1 })!, "{{items.a}}|{{lookup items 'a'}}", "1|1"),
                (JsonValue.Create(new AnnotatedItem())!, "{{items.Name}}|{{items.full_name}}", "n|")
            };

            foreach (var (value, source, expected) in cases)
            {
                Assert.Equal(expected, handlebars.Compile(source)(new JsonObject { ["items"] = value }));
            }
        }

        [Fact]
        public void MembersDoNotResolveOnScalarValues()
        {
            var template = Handlebars.Create().Compile("{{items.Length}}|{{items.Year}}");

            Assert.Equal("|", template(new JsonObject { ["items"] = "abc" }));
            Assert.Equal("|", template(new JsonObject { ["items"] = new DateTime(2020, 1, 2) }));
            Assert.Equal("|", template(Parse("{\"items\": \"abc\"}")));
        }

        [Fact]
        public void PartialHashMergesPropertiesOfValuesWrappingClrContainers()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterTemplate("partial", "{{Name}}{{a}}/{{extra}}");
            var template = handlebars.Compile("{{#with items}}{{> partial extra='x'}}{{/with}}");

            Assert.Equal("n/x", template(new JsonObject { ["items"] = JsonValue.Create(new WrappedItem()) }));
            Assert.Equal("1/x", template(new JsonObject { ["items"] = JsonValue.Create(new Dictionary<string, int> { ["a"] = 1 }) }));
        }

        [Fact]
        public void ArrayMembersUseCustomAliases()
        {
            var handlebars = Handlebars.Create();
            handlebars.Configuration.AliasProviders.Add(new DelegatedMemberAliasProvider().AddAlias<JsonArray>("itemCount", array => array.Count));
            var template = handlebars.Compile("{{arrayVal.itemCount}}");

            Assert.Equal("2", template(Parse("{\"arrayVal\": [\"a\", \"b\"]}")));
        }

        [Fact]
        public void DescriptorSupportsConcreteNodeTypesAndContainerProperties()
        {
            var provider = new JsonNodeObjectDescriptorProvider(new EnumerableObjectDescriptor(new ObjectDescriptorProvider(Array.Empty<IMemberAliasProvider>())));
            var nodes = new JsonNode[] { new JsonObject { ["stringVal"] = "a" }, new JsonArray(1), JsonValue.Create(false)! };

            foreach (var node in nodes)
            {
                Assert.True(provider.TryGetDescriptor(node.GetType(), out var descriptor));
                Assert.Equal(node.GetType(), descriptor!.DescribedType);
                var properties = descriptor.GetProperties(descriptor, node).Cast<object>().Select(property => property.ToString());
                if (node is JsonArray)
                {
                    Assert.Contains("Count", properties);
                }
                else
                {
                    Assert.Equal(node is JsonObject ? new[] { "stringVal" } : Array.Empty<string>(), properties);
                }
            }
        }

        private class WrappedItem
        {
            public string Name { get; set; } = "n";
        }

        private class AnnotatedItem
        {
            [JsonPropertyName("full_name")]
            public string Name { get; set; } = "n";
        }
    }
}
