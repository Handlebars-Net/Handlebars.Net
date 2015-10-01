using System.Dynamic;
using System.Linq;
using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class DynamicMetaObjectProviderMemberAccessor_CanHandle
    {
        private DynamicMetaObjectProviderMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DynamicMetaObjectProviderMemberAccessor();
        }

        [Test]
        public void WHEN_instance_is_DynamicObject_SHOULD_return_true()
        {
            //Arrange
            var instance = new FakeDynamicObject();

            //Act
            var canHandle = _sut.CanHandle(instance, Faker.Lorem.Words(1).First());

            //Assert
            canHandle.Should().BeTrue();
        }

        [Test]
        public void WHEN_instance_is_ExpandoObject_SHOULD_return_true()
        {
            //Arrange
            var instance = new ExpandoObject();

            //Act
            var canHandle = _sut.CanHandle(instance, Faker.Lorem.Words(1).First());

            //Assert
            canHandle.Should().BeTrue();
        }

        private class FakeDynamicObject : DynamicObject { }
    }


}
