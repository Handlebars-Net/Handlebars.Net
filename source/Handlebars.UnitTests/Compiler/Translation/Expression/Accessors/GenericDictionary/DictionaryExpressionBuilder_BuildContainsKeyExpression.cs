using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors.GenericDictionary;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors.GenericDictionary
{
    [TestFixture]
    public class DictionaryExpressionBuilder_BuildContainsKeyExpression
    {
        [Test]
        public void WHEN_contains_key_SHOULD_return_true()
        {
            //Arrange
            var key = Faker.Lorem.GetFirstWord();
            var dict = new Dictionary<string, string>
            {
                {key, Faker.Lorem.Paragraph()}
            };

            var instanceType = dict.GetType();

            //Act
            var keyExistsFunc = DictionaryExpressionBuilder.BuildContainsKeyExpression(instanceType);

            //Assert
            keyExistsFunc.Should().NotBeNull();
            keyExistsFunc.Invoke(dict, key).Should().BeTrue();
        }

        [Test]
        public void WHEN_does_not_contain_key_SHOULD_return_false()
        {
            //Arrange
            var key = Faker.Lorem.Words(1).First();
            var dict = new ConcurrentDictionary<string, string>();
            dict.AddOrUpdate(key, Faker.Lorem.Paragraph(), (s, s1) => s1);

            var instanceType = dict.GetType();

            //Act
            var keyExistsFunc = DictionaryExpressionBuilder.BuildContainsKeyExpression(instanceType);

            //Assert
            keyExistsFunc.Should().NotBeNull();
            keyExistsFunc.Invoke(dict, "").Should().BeFalse();
        }
    }
}
