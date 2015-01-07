using Handlebars.Compiler.Resolvers;
using NUnit.Framework;

namespace Handlebars.Test
{
    [TestFixture]
    public class CustomConfigurationTests
    {
        public IHandlebars Handlebars { get; private set; }
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

            this.Handlebars = new HandlebarsEnvironment(configuration);
        }

        [Test]
        [ExpectedException("System.ArgumentNullException")]
        public void InitializationWithoutConfiguration()
        {
            var handlebars = new HandlebarsEnvironment(null);
        }

        #region UpperCamelCaseExpressionNameResolver Tests

        [Test]
        public void LowerCamelCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.homeCountry}}. You're {{{description}}}.";
            var output = this.Handlebars.Compile(template).Invoke(Value);

            Assert.AreEqual(output, ExpectedOutput);
        }

        [Test]
        public void UpperCamelCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.homeCountry}}. You're {{{description}}}.";
            var output = this.Handlebars.Compile(template).Invoke(Value);

            Assert.AreEqual(output, ExpectedOutput);
        }

        [Test]
        public void SnakeCaseInputModelNaming()
        {
            var template = "Hello {{person.name}} {{person.surname}} from {{person.address.home_Country}}. You're {{{description}}}.";
            var output = this.Handlebars.Compile(template).Invoke(Value);

            Assert.AreEqual(output, ExpectedOutput);
        }

        #endregion
    }
}