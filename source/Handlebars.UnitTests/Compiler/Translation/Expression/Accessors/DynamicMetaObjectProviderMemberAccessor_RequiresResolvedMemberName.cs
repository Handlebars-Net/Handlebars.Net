using System.Dynamic;
using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class DynamicMetaObjectProviderMemberAccessor_RequiresResolvedMemberName
    {
        private DynamicMetaObjectProviderMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DynamicMetaObjectProviderMemberAccessor();
        }

        [Test]
        public void SHOULD_always_return_true()
        {
            //Arrange

            //Act
            var value = _sut.RequiresResolvedMemberName;

            //Assert
            value.Should().BeTrue();
        }
    }
}
