using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class DictionaryMemberAccessor_RequiresResolvedMemberName
    {
        private DictionaryMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new DictionaryMemberAccessor();
        }

        [Test]
        public void SHOULD_return_true()
        {
            //Arrange

            //Act
            var value = _sut.RequiresResolvedMemberName;

            //Act
            value.Should().BeTrue();
        }
    }
}
