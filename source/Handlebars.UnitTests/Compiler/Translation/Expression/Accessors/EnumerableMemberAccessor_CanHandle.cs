using System;
using System.Linq;
using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class EnumerableMemberAccessor_CanHandle
    {
        private EnumerableMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new EnumerableMemberAccessor();
        }

        [Test]
        public void WHEN_instance_is_object_SHOULD_return_false()
        {
            //Arrange
            var instance = new object();

            //Act
            var canHandle = _sut.CanHandle(instance, GetRandomIndex());

            //Assert
            canHandle.Should().BeFalse("instance is of type object and not IEnumerable<object>");
        }

        [Test]
        public void WHEN_instance_is_IEnumerableOfString_SHOULD_return_true()
        {
            //Arrange
            var instance = Enumerable.Empty<string>();

            //Act
            var canHandle = _sut.CanHandle(instance, GetRandomIndex());

            //Assert
            canHandle.Should().BeTrue();
        }
        
        [Test]
        public void WHEN_instance_is_IEnumerable_but_member_is_not_index_SHOULD_return_false()
        {
            //Arrange
            var instance = Enumerable.Empty<string>();

            //Act
            var canHandle = _sut.CanHandle(instance, Faker.Lorem.Words(1).First());

            //Assert
            canHandle.Should().BeFalse();
        }

        [Test]
        public void WHEN_instance_is_ArrayOfObject_SHOULD_return_true()
        {
            //Arrange
            var instance = new object[0];

            //Act
            var canHandle = _sut.CanHandle(instance, GetRandomIndex());

            //Assert
            canHandle.Should().BeTrue();
        }

        [Test(Description = "This test ensures IEnumerable<{any struct}> is not handled by this accessor. This should be discussed.")]
        public void WHEN_instance_is_IEnumerableOfInt_SHOULD_return_false()
        {
            //Arrange
            var instance = Enumerable.Empty<int>();

            //Act
            var canHandle = _sut.CanHandle(instance, GetRandomIndex());

            //Assert
            canHandle.Should().BeFalse();
        }

        [Test(Description = "This test ensures IEnumerable<{any struct}> is not handled by this accessor. This should be discussed.")]
        public void WHEN_instance_is_ArrayOfInt_SHOULD_return_false()
        {
            //Arrange
            var instance = new int[0];

            //Act
            var canHandle = _sut.CanHandle(instance, GetRandomIndex());

            //Assert
            canHandle.Should().BeFalse();
        }

        [Test]
        public void WHEN_instance_is_int_SHOULD_return_false()
        {
            //Arrange
            var instance = 12;

            //Act
            var canHandle = _sut.CanHandle(instance, GetRandomIndex());

            //Assert
            canHandle.Should().BeFalse("int is not an IEnumerable<object>");
        }

        private string GetRandomIndex()
        {
            var index = String.Format("[{0}]", Faker.RandomNumber.Next());
            return index;
        }
    }
}
