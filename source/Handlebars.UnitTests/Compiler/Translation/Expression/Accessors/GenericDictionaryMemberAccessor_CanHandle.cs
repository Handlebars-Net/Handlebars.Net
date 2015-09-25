using System;
using System.Collections.Generic;
using FluentAssertions;
using HandlebarsDotNet.Compiler.Translation.Expression.Accessors;
using NUnit.Framework;

namespace Handlebars.UnitTests.Compiler.Translation.Expression.Accessors
{
    [TestFixture]
    public class GenericDictionaryMemberAccessor_CanHandle
    {
        private GenericDictionaryMemberAccessor _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new GenericDictionaryMemberAccessor();
        }

        [TestCase(typeof(Dictionary<string, string>))]
        [TestCase(typeof(Dictionary<string, object>))]
        public void WHEN_instance_is_GenericDictionary_SHOULD_return_true(Type instanceType)
        {
            //Arrange
            var instance = Activator.CreateInstance(instanceType);

            //Act
            var canHandle = _sut.CanHandle(instance);

            //Assert
            canHandle.Should().BeTrue();
        }

        [TestCase(typeof(object))]
        [TestCase(typeof(List<string>))]
        public void WHEN_instance_is_not_GenericDictionary_SHOULD_return_true(Type instanceType)
        {
            //Arrange
            var instance = Activator.CreateInstance(instanceType);

            //Act
            var canHandle = _sut.CanHandle(instance);

            //Assert
            canHandle.Should().BeFalse();
        }
    }
}
