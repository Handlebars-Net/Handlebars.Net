using System.Linq;
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

            Handlebars.RegisterHelper("longAdd", (writer, context, args) =>
            {
                var arr = args.AsEnumerable().Select(a => long.Parse(a.ToString()));
                var sum = arr.Sum();
                writer.Write(sum);
            });
        }

        [Theory]
        [InlineData("{{longAdd 1000000000 9999999999}}")]
        [InlineData("{{longAdd 1000000000  9999999999}}")]
        [InlineData("{{longAdd 1000000000 9999999999 }}")]
        [InlineData("{{longAdd 1000000000    9999999999}}")]
        [InlineData("{{longAdd    1000000000    9999999999}}")]
        [InlineData("{{longAdd 1000000000 \"9999999999\"}}")]
        [InlineData("{{longAdd 1000000000 \"9999999999\" }}")]
        [InlineData("{{longAdd 1000000000    \"9999999999\"}}")]
        [InlineData("{{longAdd 1000000000    \"9999999999\" }}")]
        [InlineData("{{longAdd \"1000000000\" 9999999999}}")]
        [InlineData("{{longAdd \"1000000000\" \"9999999999\"}}")]
        public void NumericLiteralLongTests(string source)
        {
            var template = Handlebars.Compile(source);
            var data = new { };
            var result = template(data);
            Assert.Equal("10999999999", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest1()
        {
            var source = "{{numericLiteralAdd 3 4}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest2()
        {
            var source = "{{numericLiteralAdd 3  4}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest3()
        {
            var source = "{{numericLiteralAdd 3 4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest4()
        {
            var source = "{{numericLiteralAdd 3    4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest5()
        {
            var source = "{{numericLiteralAdd    3    4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest6()
        {
            var source = "{{numericLiteralAdd 3 \"4\"}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest7()
        {
            var source = "{{numericLiteralAdd 3 \"4\" }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest8()
        {
            var source = "{{numericLiteralAdd 3    \"4\" }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest9()
        {
            var source = "{{numericLiteralAdd    3   \"4\" }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest10()
        {
            var source = "{{numericLiteralAdd \"3\" 4}}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }

        [Fact]
        public void NumericLiteralIntegerTest11()
        {
            var source = "{{numericLiteralAdd \"3\" 4 }}";
            var template = Handlebars.Compile(source);
            var data = new {};
            var result = template(data);
            Assert.Equal("7", result);
        }
    }
}