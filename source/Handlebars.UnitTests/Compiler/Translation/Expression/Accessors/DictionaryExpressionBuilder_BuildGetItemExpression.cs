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
    public class DictionaryExpressionBuilder_BuildGetItemExpression
    {
        [Test]
        public void WHEN_instance_has_key_SHOULD_return_value()
        {
            //Arrange
            var key = "a";
            var value = "A";
            var dict = new Dictionary<string, string>()
            {
                {key, value}
            };

            var instanceType = dict.GetType();

            //Act
            var func = DictionaryExpressionBuilder.BuildGetItemExpression(instanceType);

            //Assert
            func.Should().NotBeNull();
            var retrievedValue = func.Invoke(dict, key);
            retrievedValue.Should().Be(value);
        }
    }
}
