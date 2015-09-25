using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class EnumerableMemberAccessor_RequiresResolvedMemberName
    {
        private EnumerableMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new EnumerableMemberAccessor();
        }

        [Test]
        public void SHOULD_always_be_false()
        {
            //Arrange

            //Act
            var value = _sut.RequiresResolvedMemberName;

            //Assert
            value.Should().BeFalse();
        }
    }
}
