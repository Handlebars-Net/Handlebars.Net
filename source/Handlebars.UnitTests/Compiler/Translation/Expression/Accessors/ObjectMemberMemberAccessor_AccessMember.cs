using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class ObjectMemberMemberAccessor_AccessMember
    {
        private static Random _random = new Randomizer();

        private ObjectMemberMemberAccessor _sut;
        private TestType _test;

        [SetUp]
        public void SetUp()
        {
            _sut = new ObjectMemberMemberAccessor();
            _test = new TestType
            {
                TestInt = Faker.RandomNumber.Next(),
                fieldInt = Faker.RandomNumber.Next(),
                TestNullableDouble = _random.NextDouble(),
                fieldNullableDouble = _random.NextDouble(),
                TestObject = new object(),
                fieldObj = new object(),
                TestStr = Faker.Lorem.Sentence(),
                fieldStr = Faker.Lorem.Sentence(),
                internalField = new object()
            };
        }

        [TestCase("TestInt")]
        [TestCase("fieldInt")]
        [TestCase("TestNullableDouble")]
        [TestCase("fieldNullableDouble")]
        [TestCase("TestObject")]
        [TestCase("fieldObj")]
        [TestCase("TestStr")]
        [TestCase("fieldStr")]
        public void WHEN_member_exists_SHOULD_return_non_null_value(string memberName)
        {
            //Arrange

            //Act
            var value = _sut.AccessMember(_test, memberName);

            //Arrange
            value.Should().NotBeNull();
        }
    }
}
