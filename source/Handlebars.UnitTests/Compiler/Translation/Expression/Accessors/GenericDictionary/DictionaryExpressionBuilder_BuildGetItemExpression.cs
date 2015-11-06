using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors.GenericDictionary;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors.GenericDictionary
{
    [TestFixture]
    public class DictionaryExpressionBuilder_BuildGetItemExpression
    {
        [Test]
        public void WHEN_instance_has__string_key_SHOULD_return_string_value()
        {
            //Arrange
            var key = "a";
            var value = Faker.Lorem.Sentence();
            var dict = new Dictionary<string, string>()
            {
                {key, value}
            };

            var instanceType = dict.GetType();

            //Act
            var func = DictionaryExpressionBuilder.BuildGetItemExpression(instanceType);
            var retrievedValue = func.Invoke(dict, key);

            //Assert
            retrievedValue.Should().Be(value);
        }

        [Test]
        public void WHEN_instance_has_int_key_SHOULD_return_object_value()
        {
            //Arrange
            var key = Faker.RandomNumber.Next();
            var value = new StringBuilder(Faker.Company.CatchPhrase());
            var dict = new Dictionary<int, StringBuilder>()
            {
                {key, value}
            };

            var instanceType = dict.GetType();

            //Act
            var func = DictionaryExpressionBuilder.BuildGetItemExpression(instanceType);
            var retrievedValue = func.Invoke(dict, key);

            //Assert
            retrievedValue.Should().Be(value);
        }

        [Test]
        public void WHEN_instance_has_int_key_SHOULD_return_int_value()
        {
            //Arrange
            var key = Faker.RandomNumber.Next();
            var value = Faker.RandomNumber.Next();
            var dict = new Dictionary<int, int>()
            {
                {key, value}
            };

            var instanceType = dict.GetType();

            //Act
            var func = DictionaryExpressionBuilder.BuildGetItemExpression(instanceType);
            var retrievedValue = func.Invoke(dict, key);

            //Assert
            retrievedValue.Should().Be(value);
        }

        [Test]
        public void WHEN_instance_does_not_have_key_SHOULD_throw_KeyNotFoundException()
        {
            //Arrange
            var key = "a";
            var dict = new Dictionary<string, object>()
            {
                {"b", Faker.Lorem.Sentence()}
            };

            var instanceType = dict.GetType();

            //Act
            var func = DictionaryExpressionBuilder.BuildGetItemExpression(instanceType);

            //Assert
            var ex = Assert.Throws<KeyNotFoundException>(() => func.Invoke(dict, key));
            ex.Should().NotBeNull();
        }


    }
}
