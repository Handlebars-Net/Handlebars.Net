using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class DictionaryMemberAccessor_AccessMember
    {
        private DictionaryMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DictionaryMemberAccessor();
        }

        [Test]
        public void WHEN_instance_contains_key_SHOULD_return_value()
        {
            //Arrange
            var instance = new Dictionary<string, int>
            {
                {"A", 1},
                {"B", 2},
                {"C", 3},
            };

            var key = instance.Keys.First();
            var value = instance[key];

            //Act
            var resolvedValue = _sut.AccessMember(instance, key);

            //Assert
            resolvedValue.Should().Be(value);
        }

        [Test]
        public void WHEN_instance_does_not_contain_key_SHOULD_return_UndefinedBindingResult()
        {
            //Arrange
            var instance = new Dictionary<string, int>
            {
                {"A", 1},
                {"B", 2},
                {"C", 3},
            };

            var key = "ABC";

            //Act
            var resolvedValue = _sut.AccessMember(instance, key);

            //Assert
            resolvedValue.Should().BeAssignableTo<UndefinedBindingResult>();
        }
    }
}
