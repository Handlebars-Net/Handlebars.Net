using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class ObjectMemberMemberAccessor_CanHandle
    {
        private ObjectMemberMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ObjectMemberMemberAccessor();
        }

        [Test]
        public void SHOULD_return_true()
        {
            //Arrange
            var instance = new TestType();

            //Act
            var canHandle = _sut.CanHandle(instance);

            //Assert
            canHandle.Should().BeTrue();
        }
    }
}
