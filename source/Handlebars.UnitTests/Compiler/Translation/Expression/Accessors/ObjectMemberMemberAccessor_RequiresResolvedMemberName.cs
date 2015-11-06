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
    public class ObjectMemberMemberAccessor_RequiresResolvedMemberName
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

            //Act
            var value = _sut.RequiresResolvedMemberName;

            //Assert
            value.Should().BeTrue();
        }
    }
}
