using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors.Members;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors.Members
{
    [TestFixture]
    public class MemberExpressionBuilder_BuildMemberGetterFunc
    {
        [Test]
        public void WHEN_property_is_string_SHOULD_return_func()
        {
            //Arrange
            var instance = new TestType
            {
                TestStr = Faker.Lorem.Sentence()
            };

            //Act
            var func = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(typeof(TestType), "TestStr");

            //Assert
            var value = func.Invoke(instance);
            value.Should().Be(instance.TestStr);
        }

        [Test]
        public void WHEN_field_is_string_SHOULD_return_func()
        {
            //Arrange
            var instance = new TestType
            {
                fieldStr = Faker.Lorem.Sentence()
            };

            //Act
            var func = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(typeof(TestType), "fieldStr");

            //Assert
            var value = func.Invoke(instance);
            value.Should().Be(instance.fieldStr);
        }

        [Test]
        public void WHEN_property_is_int_SHOULD_return_func()
        {
            //Arrange
            var instance = new TestType
            {
                TestInt = Faker.RandomNumber.Next()
            };

            //Act
            var func = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(typeof(TestType), "TestInt");

            //Assert
            var value = func.Invoke(instance);
            value.Should().Be(instance.TestInt);
        }

        [Test]
        public void WHEN_field_is_int_SHOULD_return_func()
        {
            //Arrange
            var instance = new TestType
            {
                fieldInt = Faker.RandomNumber.Next()
            };

            //Act
            var func = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(typeof(TestType), "fieldInt");

            //Assert
            var value = func.Invoke(instance);
            value.Should().Be(instance.fieldInt);
        }

        [TestCase(12.584)]
        [TestCase(null)]
        public void WHEN_property_is_nullable_double_SHOULD_return_func(double? value)
        {
            //Arrange
            var instance = new TestType
            {
                TestNullableDouble = value
            };

            //Act
            var func = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(typeof(TestType), "TestNullableDouble");

            //Assert
            var retrievedValue = func.Invoke(instance);
            retrievedValue.Should().Be(instance.TestNullableDouble);
        }

        [TestCase(12.584)]
        [TestCase(null)]
        public void WHEN_field_is_nullable_double_SHOULD_return_func(double? value)
        {
            //Arrange
            var instance = new TestType
            {
                fieldNullableDouble = value
            };

            //Act
            var func = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(typeof(TestType), "fieldNullableDouble");

            //Assert
            var retrievedValue = func.Invoke(instance);
            retrievedValue.Should().Be(instance.fieldNullableDouble);
        }

        [TestCase("protectedField")]
        [TestCase("privateField")]
        [TestCase("internalField")]
        [TestCase("nonExistingField")]
        [TestCase("NonExistingProperty")]
        public void WHEN_field_is_unaccessible_SHOULD_return_null(string memberName)
        {
            //Arrange

            //Act
            var func = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(typeof (TestType), memberName);

            //Assert
            func.Should().BeNull();
        }

        [Test]
        public void WHEN_property_is_object_SHOULD_return_func()
        {
            //Arrange
            var instance = new TestType
            {
                TestObject = new object()
            };

            //Act
            var func = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(typeof(TestType), "TestObject");

            //Assert
            var value = func.Invoke(instance);
            value.Should().Be(instance.TestObject);
        }

        [Test]
        public void WHEN_field_is_object_SHOULD_return_func()
        {
            //Arrange
            var instance = new TestType
            {
                fieldObj = new object()
            };

            //Act
            var func = ObjectMemberExpressionBuilder.BuildMemberGetterFunc(typeof(TestType), "fieldObj");

            //Assert
            var value = func.Invoke(instance);
            value.Should().Be(instance.fieldObj);
        }
    }
}
