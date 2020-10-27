using System;
using System.Globalization;
using System.IO;
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

            Assert.Equal(output, ExpectedOutput);
        }

        [Fact]
        public void UpperCamelCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.HomeCountry}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Invoke(Value);

            Assert.Equal(output, ExpectedOutput);
        }

        [Fact]
        public void SnakeCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.home_Country}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Invoke(Value);

            Assert.Equal(output, ExpectedOutput);
        }

        #endregion

        #region Custom IOutputEncoding

        private class JsonEncoder : ITextEncoder
        {
            public void Encode(StringBuilder text, TextWriter target)
            {
                target.Write(JsonConvert.ToString(text.ToString(), '"').Trim('"'));
            }

            public void Encode(string text, TextWriter target)
            {
                target.Write(JsonConvert.ToString(text, '"').Trim('"'));
            }

            public bool ShouldEncode(char c) => true;

            public IFormatProvider FormatProvider { get; } = CultureInfo.InvariantCulture;
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
