using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class DictionaryMemberAccessors_CanHandle
    {
        private DictionaryMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DictionaryMemberAccessor();
        }

        [Test]
        public void WHEN_instance_is_IDictionary_SHOULD_return_true()
        {
            //Arrange
            var instance = new ListDictionary();

            //Act
            var value = _sut.CanHandle(instance, Faker.Lorem.Words(1).First());

            //Assert
            value.Should().BeTrue();
        }

        [Test]
        public void WHEN_instance_is_GenericDictionary_SHOULD_return_true()
        {
            //Arrange
            var instance = new Dictionary<string, string>();

            //Act
            var value = _sut.CanHandle(instance, Faker.Lorem.Words(1).First());

            //Assert
            value.Should().BeTrue();
        }

        [Test]
        public void WHEN_instance_is_Object_SHOULD_return_false()
        {
            //Arrange
            var instance = new object();

            //Act
            var value = _sut.CanHandle(instance, Faker.Lorem.Words(1).First());

            //Assert
            value.Should().BeFalse();
        }
    }
}
