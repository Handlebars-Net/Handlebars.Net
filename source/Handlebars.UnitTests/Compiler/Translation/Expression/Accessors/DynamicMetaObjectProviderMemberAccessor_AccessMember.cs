using System.Dynamic;
using FluentAssertions;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class DynamicMetaObjectProviderMemberAccessor_AccessMember
    {
        private DynamicMetaObjectProviderMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DynamicMetaObjectProviderMemberAccessor();
        }

        [Test]
        public void WHEN_instance_has_property_SHOULD_return_value()
        {
            //Arrange
            var value = Faker.Lorem.Sentence();

            dynamic instance = new ExpandoObject();
            instance.Test = value;

            var memberName = "Test";

            //Act
            object memberValue = _sut.AccessMember(instance, memberName);

            //Assert
            memberValue.Should().NotBeNull();
            memberValue.Should().Be(value);
        }

        [Test]
        public void WHEN_instance_doesnt_have_property_SHOULD_return_UndefinedBindingResult()
        {
            //Arrange
            var value = Faker.Lorem.Sentence();

            dynamic instance = new ExpandoObject();
            instance.Test = value;

            var memberName = "NonExistingProperty";

            //Act
            object memberValue = _sut.AccessMember(instance, memberName);

            //Assert
            memberValue.Should().NotBeNull();
            memberValue.Should().BeAssignableTo<UndefinedBindingResult>();
        }
    }
}
