using System;
using System.Linq;
using NUnit.Framework;

namespace HandlebarsDotNet.Test
{
    [TestFixture]
    public class NumericLiteralTests
    {
        public NumericLiteralTests()
        {
            Handlebars.RegisterHelper("numericLiteralAdd", (writer, context, args) =>
                {
                    args = args.Select(a => (object)int.Parse((string)a)).ToArray();
                    writer.Write(args.Aggregate(0, (a, i) => a + (int)i));
                });
        }

        [Test]
        public void NumericLiteralTest1()
        {
            var source = "{{numericLiteralAdd 3 4}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }

        [Test]
        public void NumericLiteralTest2()
        {
            var source = "{{numericLiteralAdd 3  4}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }

        [Test]
        public void NumericLiteralTest3()
        {
            var source = "{{numericLiteralAdd 3 4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }

        [Test]
        public void NumericLiteralTest4()
        {
            var source = "{{numericLiteralAdd 3    4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }

        [Test]
        public void NumericLiteralTest5()
        {
            var source = "{{numericLiteralAdd    3    4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }

        [Test]
        public void NumericLiteralTest6()
        {
            var source = "{{numericLiteralAdd 3 \"4\"}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }

        [Test]
        public void NumericLiteralTest7()
        {
            var source = "{{numericLiteralAdd 3 \"4\" }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }

        [Test]
        public void NumericLiteralTest8()
        {
            var source = "{{numericLiteralAdd 3    \"4\" }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }

        [Test]
        public void NumericLiteralTest9()
        {
            var source = "{{numericLiteralAdd    3   \"4\" }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }

        [Test]
        public void NumericLiteralTest10()
        {
            var source = "{{numericLiteralAdd \"3\" 4}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }
        [Test]
        public void NumericLiteralTest11()
        {
            var source = "{{numericLiteralAdd \"3\" 4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.AreEqual("7", result);
        }
    }
}

