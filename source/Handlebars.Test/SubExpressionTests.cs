using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class SubExpressionTests
    {
        [Fact]
        public void BasicSubExpression()
        {
            var helperName = "helper-" + Guid.NewGuid().ToString(); //randomize helper name
            var subHelperName = "subhelper-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(helperName, (writer, context, args) => {
                writer.Write("Hello " + args[0]);
            });

            Handlebars.RegisterHelper(subHelperName, (writer, context, args) => {
                writer.Write("world");
            });

            var source = "{{" + helperName + " (" + subHelperName + ")}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Hello world";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void BasicSubExpressionWithStringLiteralArgument()
        {
            var helperName = "helper-" + Guid.NewGuid().ToString(); //randomize helper name
            var subHelperName = "subhelper-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(helperName, (writer, context, args) => {
                writer.Write("Outer " + args[0]);
            });

            Handlebars.RegisterHelper(subHelperName, (writer, context, args) => {
                writer.Write("Inner " + args[0]);
            });

            var source = "{{" + helperName + " (" + subHelperName + " 'inner-arg')}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Outer Inner inner-arg";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void BasicSubExpressionWithHashArgument()
        {
            var handlebars = Handlebars.Create();
            
            handlebars.RegisterHelper("helper", (writer, context, args) => {
                writer.Write("Outer " + args[0]);
            });

            handlebars.RegisterHelper("subhelper", (writer, context, args) => {
                var hash = args[0] as Dictionary<string, object>;
                writer.Write("Inner " + hash["item1"] + "-" + hash["item2"]);
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
            var helperName = "helper-" + Guid.NewGuid().ToString(); //randomize helper name
            var subHelperName = "subhelper-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(helperName, (writer, context, args) => {
                writer.Write("Math " + args[0]);
            });

            Handlebars.RegisterHelper(subHelperName, (writer, context, args) => {
                writer.Write((int)args[0] + (int)args[1]);
            });

            var source = "{{" + helperName + " (" + subHelperName + " 1 2)}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Math 3";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void BasicSubExpressionWithPathArgument()
        {
            var helperName = "helper-" + Guid.NewGuid().ToString(); //randomize helper name
            var subHelperName = "subhelper-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(helperName, (writer, context, args) => {
                writer.Write("Outer " + args[0]);
            });

            Handlebars.RegisterHelper(subHelperName, (writer, context, args) => {
                writer.Write("Inner " + args[0]);
            });

            var source = "{{" + helperName + " (" + subHelperName + " property)}}";

            var template = Handlebars.Compile(source);

            var output = template(new { 
                property = "inner-arg"
            });

            var expected = "Outer Inner inner-arg";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void TwoBasicSubExpressionsWithNumericLiteralArguments()
        {
            var mathHelper = "math-" + Guid.NewGuid().ToString(); //randomize helper name
            var addHelper = "add-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(mathHelper, (writer, context, args) => {
                writer.Write("Math " + args[0] + " " + args[1]);
            });

            Handlebars.RegisterHelper(addHelper, (writer, context, args) => {
                writer.Write((int)args[0] + (int)args[1]);
            });

            var source = "{{" + mathHelper + " (" + addHelper + " 1 2) (" + addHelper + " 3 4)}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Math 3 7";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void BasicSubExpressionWithNumericAndStringLiteralArguments()
        {
            var writeHelper = "write-" + Guid.NewGuid().ToString(); //randomize helper name
            var addHelper = "add-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(writeHelper, (writer, context, args) => {
                writer.Write(args[0] + " " + args[1]);
            });

            Handlebars.RegisterHelper(addHelper, (writer, context, args) => {
                writer.Write((int)args[0] + (int)args[1]);
            });

            var source = "{{" + writeHelper + " (" + addHelper + " 1 2) \"hello\"}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "3 hello";

            Assert.Equal(expected, output);
        }

        [Fact]
        public void NestedSubExpressionsWithNumericLiteralArguments()
        {
            var writeHelper = "write-" + Guid.NewGuid().ToString(); //randomize helper name
            var addHelper = "add-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(writeHelper, (writer, context, args) => {
                writer.Write(args[0]);
            });

            Handlebars.RegisterHelper(addHelper, (writer, context, args) => {
                args = args.Select(a => (object)int.Parse(a.ToString())).ToArray();
                writer.Write((int)args[0] + (int)args[1]);
            });

            var source = "{{" + writeHelper + " (" + addHelper + " (" + addHelper + " 1 2) 3 )}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "6";

            Assert.Equal(expected, output);
        }
    }
}

