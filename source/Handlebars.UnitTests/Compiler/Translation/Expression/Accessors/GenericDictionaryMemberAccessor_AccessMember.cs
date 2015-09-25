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
    public class GenericDictionaryMemberAccessor_AccessMember
    {
        delegate bool CK(object key);

        private GenericDictionaryMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new GenericDictionaryMemberAccessor();
        }

        [Test]
        public void WHEN_instance_is_key_an_int_SHOULD_return_value()
        {
            //Arrange
            var dict = new Dictionary<int, object>()
            {
                { 0, new object()},
                { 1, new object()}
            };

            var key = "0";

            //Act
            var value = _sut.AccessMember(dict, key);

            //Assert
            value.Should().NotBeNull();
            value.Should().BeAssignableTo<int>();
        }
    }
}
