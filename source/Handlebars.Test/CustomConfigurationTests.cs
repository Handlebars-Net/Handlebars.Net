using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using HandlebarsDotNet.Compiler.Resolvers;
using Newtonsoft.Json;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class CustomConfigurationTests
    {
        public IHandlebars HandlebarsInstance { get; private set; }
        public const string ExpectedOutput = "Hello Eric Sharp from Japan. You're <b>AWESOME</b>.";
        public object Value = new
                    {
                        Person = new { Name = "Eric", Surname = "Sharp", Address = new { HomeCountry = "Japan" } },
                        Description = @"<b>AWESOME</b>"
                    };

        public CustomConfigurationTests()
        {
            var configuration = new HandlebarsConfiguration
                                    {
                                        ExpressionNameResolver =
                                            new UpperCamelCaseExpressionNameResolver()
                                    };
                        
            this.HandlebarsInstance = Handlebars.Create(configuration);
        }

        #region UpperCamelCaseExpressionNameResolver Tests

        [Fact]
        public void LowerCamelCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.homeCountry}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Invoke(Value);

            Assert.Equal(ExpectedOutput, output);
        }

        [Fact]
        public void UpperCamelCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.HomeCountry}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Invoke(Value);

            Assert.Equal(ExpectedOutput, output);
        }

        [Fact]
        public void SnakeCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.home_Country}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Invoke(Value);

            Assert.Equal(ExpectedOutput, output);
        }

        [Fact]
        public void UpperCamelCaseResolverDoesNotBreakEachIteration()
        {
            var template = HandlebarsInstance.Compile("{{#each items}}{{name}} {{/each}}");
            var data = new { items = new[] { new { name = "Alice" }, new { name = "Bob" } } };
            Assert.Equal("Alice Bob ", template(data));
        }

        [Fact]
        public void UpperCamelCaseResolverDoesNotBreakEachWithList()
        {
            var template = HandlebarsInstance.Compile("{{#each items}}{{name}} {{/each}}");
            var data = new
            {
                items = new List<object>
                {
                    new { name = "Alice" },
                    new { name = "Bob" }
                }
            };
            Assert.Equal("Alice Bob ", template(data));
        }

        [Fact]
        public void UpperCamelCaseResolverDoesNotBreakEachWithAtIndex()
        {
            var template = HandlebarsInstance.Compile("{{#each items}}{{@index}}:{{name}} {{/each}}");
            var data = new { items = new[] { new { name = "Alice" }, new { name = "Bob" } } };
            Assert.Equal("0:Alice 1:Bob ", template(data));
        }

        [Fact]
        public void UpperCamelCaseResolverDoesNotBreakEachWithAtFirst()
        {
            var template = HandlebarsInstance.Compile("{{#each items}}{{#if @first}}first:{{/if}}{{name}} {{/each}}");
            var data = new { items = new[] { new { name = "Alice" }, new { name = "Bob" } } };
            Assert.Equal("first:Alice Bob ", template(data));
        }

        [Fact]
        public void UpperCamelCaseResolverWorksWithNestedPropertyAccess()
        {
            var template = HandlebarsInstance.Compile("{{#each items}}{{address.city}} {{/each}}");
            var data = new
            {
                items = new[]
                {
                    new { address = new { city = "New York" } },
                    new { address = new { city = "London" } }
                }
            };
            Assert.Equal("New York London ", template(data));
        }

        [Fact]
        public void UpperCamelCaseResolverWorksWithStringArray()
        {
            var template = HandlebarsInstance.Compile("{{#each items}}{{this}} {{/each}}");
            var data = new { items = new[] { "Alice", "Bob" } };
            Assert.Equal("Alice Bob ", template(data));
        }

        [Fact]
        public void UpperCamelCaseResolverWorksWithNestedEach()
        {
            var template = HandlebarsInstance.Compile("{{#each groups}}{{#each members}}{{name}} {{/each}}{{/each}}");
            var data = new
            {
                groups = new[]
                {
                    new { members = new[] { new { name = "Alice" }, new { name = "Bob" } } },
                    new { members = new[] { new { name = "Carol" } } }
                }
            };
            Assert.Equal("Alice Bob Carol ", template(data));
        }

        #endregion

        #region Custom IOutputEncoding

        private class JsonEncoder : ITextEncoder
        {
            public void Encode(StringBuilder? text, TextWriter target)
            {
                target.Write(JsonConvert.ToString(text?.ToString(), '"').Trim('"'));
            }

            public void Encode(string? text, TextWriter target)
            {
                target.Write(JsonConvert.ToString(text, '"').Trim('"'));
            }

            public void Encode<T>(T? text, TextWriter target) where T : IEnumerator<char>
            {
                Encode(new string(new Adapter<T, char>(text!).ToArray()), target);
            }

            public IFormatProvider FormatProvider { get; } = CultureInfo.InvariantCulture;
            
            private class Adapter<T, TV> : IEnumerable<TV>
                where T: IEnumerator<TV>
            {
                private readonly T _enumerator;

                public Adapter(T enumerator) => _enumerator = enumerator;

                public IEnumerator<TV> GetEnumerator() => _enumerator;

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }
        }


        [Fact]
        public void NoOutputEncoding()
        {
            var template =
                "Hello {{person.name}} {{person.surname}} from {{person.address.homeCountry}}. You're {{description}}.";


            var configuration = new HandlebarsConfiguration
                                    {
                                        TextEncoder = null
                                    };

            var handlebarsInstance = Handlebars.Create(configuration);

            var output = handlebarsInstance.Compile(template).Invoke(Value);

            Assert.Equal(ExpectedOutput, output);
        }

        [Fact]
        public void JsonEncoding()
        {
            var template = "No html entities, {{Username}}.";


            var configuration = new HandlebarsConfiguration
                                    {
                                        TextEncoder = new JsonEncoder()
                                    };

            var handlebarsInstance = Handlebars.Create(configuration);

            var value = new {Username = "\"<Eric>\"\n<Sharp>"};
            var output = handlebarsInstance.Compile(template).Invoke(value);

            Assert.Equal(@"No html entities, \""<Eric>\""\n<Sharp>.", output);
        }

        #endregion
    }
}
