using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace HandlebarsDotNet.Test
{
    [TestFixture]
    public class SubExpressionTests
    {
        [Test]
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

            Assert.AreEqual(expected, output);
        }

        [Test]
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

            Assert.AreEqual(expected, output);
        }

        [Test]
        public void BasicSubExpressionWithHashArgument()
        {
            var helperName = "helper-" + Guid.NewGuid().ToString(); //randomize helper name
            var subHelperName = "subhelper-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(helperName, (writer, context, args) => {
                writer.Write("Outer " + args[0]);
            });

            Handlebars.RegisterHelper(subHelperName, (writer, context, args) => {
                var hash = args[0] as Dictionary<string, object>;
                writer.Write("Inner " + hash["item1"] + "-" + hash["item2"]);
            });

            var source = "{{" + helperName + " (" + subHelperName + " item1='inner' item2='arg')}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Outer Inner inner-arg";

            Assert.AreEqual(expected, output);
        }

        [Test]
        public void BasicSubExpressionWithNumericLiteralArguments()
        {
            var helperName = "helper-" + Guid.NewGuid().ToString(); //randomize helper name
            var subHelperName = "subhelper-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(helperName, (writer, context, args) => {
                writer.Write("Math " + args[0]);
            });

            Handlebars.RegisterHelper(subHelperName, (writer, context, args) => {
                args = args.Select(a => (object)int.Parse((string)a)).ToArray();
                writer.Write((int)args[0] + (int)args[1]);
            });

            var source = "{{" + helperName + " (" + subHelperName + " 1 2)}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Math 3";

            Assert.AreEqual(expected, output);
        }

        [Test]
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

            Assert.AreEqual(expected, output);
        }
    }
}

