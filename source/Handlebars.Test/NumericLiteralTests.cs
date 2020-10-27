using System;
using System.Linq;
using HandlebarsDotNet.Compiler;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class NumericLiteralTests
    {
        public NumericLiteralTests()
        {
            Handlebars.RegisterHelper("numericLiteralAdd", (writer, context, args) =>
                {
                    var arr = args.AsEnumerable().Select(a => (object)int.Parse(a.ToString()));
                    writer.Write(arr.Aggregate(0, (a, i) => a + (int)i));
                });
        }

        [Fact]
        public void NumericLiteralTest1()
        {
            var source = "{{numericLiteralAdd 3 4}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest2()
        {
            var source = "{{numericLiteralAdd 3  4}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest3()
        {
            var source = "{{numericLiteralAdd 3 4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest4()
        {
            var source = "{{numericLiteralAdd 3    4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest5()
        {
            var source = "{{numericLiteralAdd    3    4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest6()
        {
            var source = "{{numericLiteralAdd 3 \"4\"}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest7()
        {
            var source = "{{numericLiteralAdd 3 \"4\" }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest8()
        {
            var source = "{{numericLiteralAdd 3    \"4\" }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest9()
        {
            var source = "{{numericLiteralAdd    3   \"4\" }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest10()
        {
            var source = "{{numericLiteralAdd \"3\" 4}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralTest11()
        {
            var source = "{{numericLiteralAdd \"3\" 4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }
    }
}

