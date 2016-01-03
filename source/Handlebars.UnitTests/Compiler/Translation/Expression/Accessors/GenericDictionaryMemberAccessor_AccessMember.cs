using System.Collections.Generic;
using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class GenericDictionaryMemberAccessor_AccessMember
    {
        private GenericDictionaryMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new GenericDictionaryMemberAccessor();
        }

        [Test]
        public void WHEN_key_is_int_SHOULD_return_value()
        {
            //Arrange
            var dict = new Dictionary<int, int>()
            {
                { 0, 0},
                { 1, 1}
            };

            var key = "0";

            //Act
            var value = _sut.AccessMember(dict, key);

            //Assert
            value.Should().NotBeNull();
            value.Should().BeAssignableTo<int>();
        }

        [Test]
        public void WHEN_calling_twice_on_same_type_SHOULD_not_build_more_than_once()
        {
            //Arrange
            var dict = new Dictionary<int, int>()
            {
                { 1, 1 },
                { 2, 2 }
            };

            //Act
            var value = _sut.AccessMember(dict, "1");
            value = _sut.AccessMember(dict, "2");

            //Assert
            value.Should().NotBeNull();
            value.Should().NotBe(0);
        }
    }
}
