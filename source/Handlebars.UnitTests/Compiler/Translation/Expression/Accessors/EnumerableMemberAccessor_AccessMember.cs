using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class EnumerableMemberAccessor_AccessMember
    {
        private EnumerableMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new EnumerableMemberAccessor();
        }

        [Test]
        public void WHEN_instance_is_filled_list_SHOULD_return_value_at_index()
        {
            //Arrange
            var instance = new List<string>()
            {
                Faker.Lorem.GetFirstWord(),
                Faker.Lorem.GetFirstWord(),
                Faker.Lorem.GetFirstWord()
            };

            var index = Faker.RandomNumber.Next(0, instance.Count - 1);
            var indexStr = index.ToString();

            //Act
            var value = _sut.AccessMember(instance, indexStr);

            //Assert
            value.Should().Be(instance[index]);
        }

        [Test]
        public void WHEN_index_is_out_of_bound_SHOULD_return_UndefinedBindingResult()
        {
            //Arrange
            var instance = Enumerable.Empty<object>();

            var index = Faker.RandomNumber.Next(1, int.MaxValue);
            var indexStr = index.ToString();

            //Act
            var result = _sut.AccessMember(instance, indexStr);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<UndefinedBindingResult>();
        }

        [TestCase("")]
        [TestCase("     ")]
        [TestCase("3.1416")]
        [TestCase("20A")]
        [TestCase("HelloWorld!")]
        public void WHEN_index_is_not_an_int_SHOULD_return_UndefinedBindingResult(object index)
        {
            //Arrange
            var instance = Enumerable.Empty<object>();

            var indexStr = index.ToString();

            //Act
            var result = _sut.AccessMember(instance, indexStr);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<UndefinedBindingResult>();
        }
    }
}
