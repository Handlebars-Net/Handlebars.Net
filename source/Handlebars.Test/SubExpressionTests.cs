using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class SubExpressionTests
    {
        [Fact]
        public void BasicSubExpression()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("helper", (context, args) => "Hello " + args[0]);

            handlebars.RegisterHelper("subhelper", (context, args) => "world");

            var source = "{{helper (subhelper)}}";

            var template = handlebars.Compile(source);

            var output = template(new { });

            var expected = "Hello world";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void BasicSubExpressionWithStringLiteralArgument()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("helper", (writer, context, args) => {
                writer.Write($"Outer {args[0]}");
            });

            handlebars.RegisterHelper("subhelper", (writer, context, args) => {
                writer.Write($"Inner {args[0]}");
            });

            var source = "{{helper (subhelper 'inner-arg')}}";

            var template = handlebars.Compile(source);

            var output = template(new { });

            var expected = "Outer Inner inner-arg";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void BasicSubExpressionWithHashArgument()
        {
            var handlebars = Handlebars.Create();
            
            handlebars.RegisterHelper("helper", (writer, context, args) => {
                writer.Write($"Outer {args[0]}");
            });

            handlebars.RegisterHelper("subhelper", (writer, context, args) =>
            {
                writer.Write($"Inner {args["item1"]}-{args["item2"]}");
            });

            var source = "{{ helper (subhelper item1='inner' item2='arg')}}";

            var template = handlebars.Compile(source);

            var output = template(new { });

            var expected = "Outer Inner inner-arg";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void BasicSubExpressionWithNumericLiteralArguments()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("helper", (writer, context, args) => writer.Write($"Math {args[0]}"));
            handlebars.RegisterHelper("subhelper", (writer, context, args) => writer.Write((int)args[0] + (int)args[1]));

            var source = "{{helper (subhelper 1 2)}}";

            var template = handlebars.Compile(source);

            var output = template(new { });

            var expected = "Math 3";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void BasicSubExpressionWithPathArgument()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("helper", (writer, context, args) => writer.Write($"Outer {args[0]}"));
            handlebars.RegisterHelper("subhelper", (writer, context, args) => writer.Write($"Inner {args[0]}"));

            var source = "{{helper (subhelper property)}}";

            var template = handlebars.Compile(source);

            var output = template(new { 
                property = "inner-arg"
            });

            var expected = "Outer Inner inner-arg";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void TwoBasicSubExpressionsWithNumericLiteralArguments()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("math", (writer, context, args) => {
                writer.Write("Math " + args[0] + " " + args[1]);
            });

            handlebars.RegisterHelper("add", (writer, context, args) => {
                writer.Write((int)args[0] + (int)args[1]);
            });

            var source = "{{math (add 1 2) (add 3 4)}}";

            var template = handlebars.Compile(source);

            var output = template(new { });

            var expected = "Math 3 7";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void BasicSubExpressionWithNumericAndStringLiteralArguments()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("write", (writer, context, args) => {
                writer.Write(args[0] + " " + args[1]);
            });

            handlebars.RegisterHelper("add", (writer, context, args) => {
                writer.Write((int)args[0] + (int)args[1]);
            });

            var source = "{{write (add 1 2) \"hello\"}}";

            var template = handlebars.Compile(source);

            var output = template(new { });

            var expected = "3 hello";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void NestedSubExpressionsWithNumericLiteralArguments()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterHelper("write", (writer, context, args) => {
                writer.Write(args[0]);
            });

            handlebars.RegisterHelper("add", (context, args) 
                => args.At<int>(0) + args.At<int>(1));

            var source = "{{write (add (add 1 2) 3 )}}";

            var template = handlebars.Compile(source);

            var output = template(new { });

            var expected = "6";

            Assert.Equal(expected, output);
        }
    }
}

