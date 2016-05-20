using System;
using HandlebarsDotNet.Compiler.Resolvers;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HandlebarsDotNet.Test
{
    [TestFixture]
    public class CustomConfigurationTests
    {
        public IHandlebars HandlebarsInstance { get; private set; }
        public const string ExpectedOutput = "Hello Eric Sharp from Japan. You're <b>AWESOME</b>.";
        public object Value = new
                    {
                        Person = new { Name = "Eric", Surname = "Sharp", Address = new { HomeCountry = "Japan" } },
                        Description = @"<b>AWESOME</b>"
                    };

        [TestFixtureSetUp]
        public void Init()
        {
            var configuration = new HandlebarsConfiguration
                                    {
                                        ExpressionNameResolver =
                                            new UpperCamelCaseExpressionNameResolver()
                                    };
                        
            this.HandlebarsInstance = Handlebars.Create(configuration);
        }

        #region UpperCamelCaseExpressionNameResolver Tests

        [Test]
        public void LowerCamelCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.homeCountry}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Invoke(Value);

            Assert.AreEqual(output, ExpectedOutput);
        }

        [Test]
        public void UpperCamelCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.homeCountry}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Invoke(Value);

            Assert.AreEqual(output, ExpectedOutput);
        }

        [Test]
        public void SnakeCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.home_Country}}. You're {{{description}}}.";
            var output = this.HandlebarsInstance.Compile(template).Invoke(Value);

            Assert.AreEqual(output, ExpectedOutput);
        }

        #endregion

        #region Custom IOutputEncoding

        private class JsonEncoder : ITextEncoder
        {
            public string Encode(string value)
            {
                return value != null ? JsonConvert.ToString(value, '"').Trim('"') : String.Empty;
            }
        }


        [Test]
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

            Assert.AreEqual(ExpectedOutput, output);
        }

        [Test]
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

            Assert.AreEqual(@"No html entities, \""<Eric>\""\n<Sharp>.", output);
        }

        #endregion
    }
}