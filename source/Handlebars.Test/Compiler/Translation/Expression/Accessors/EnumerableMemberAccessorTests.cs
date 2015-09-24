using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.Test.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class EnumerableMemberAccessorTests
    {
        private EnumerableMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new EnumerableMemberAccessor();
        }

        [Test]
        public void CanHandleShouldReturnFalseWhenInstanceIsObject()
        {
            //Arrange
            var instance = new object();

            //Act
            var canHandle = _sut.CanHandle(instance);

            //Assert
            Assert.IsFalse(canHandle);
        }


    }
}
