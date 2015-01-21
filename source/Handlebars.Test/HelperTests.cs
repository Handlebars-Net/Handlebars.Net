using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using System;
using Handlebars;

namespace Handlebars.Test
{
    [TestFixture]
    public class HelperTests
    {
        [Test]
        public void HelperWithLiteralArguments()
        {
            Handlebars.RegisterHelper("myHelper", (writer, context, args) => {
                var count = 0;
                foreach(var arg in args)
                {
                    writer.Write("\nThing {0}: {1}", ++count, arg);
                }
            });

            var source = "Here are some things: {{myHelper 'foo' 'bar'}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Here are some things: \nThing 1: foo\nThing 2: bar";

            Assert.AreEqual(expected, output);
        }

        [Test]
        public void HelperWithLiteralArgumentsWithQuotes()
        {
            var helperName = "helper-" + Guid.NewGuid().ToString(); //randomize helper name
            Handlebars.RegisterHelper(helperName, (writer, context, args) => {
                var count = 0;
                foreach(var arg in args)
                {
                    writer.WriteSafeString(
                        string.Format("\nThing {0}: {1}", ++count, arg));
                }
            });

            var source = "Here are some things: {{" + helperName + " 'My \"favorite\" movie' 'bar'}}";

            var template = Handlebars.Compile(source);

            var output = template(new { });

            var expected = "Here are some things: \nThing 1: My \"favorite\" movie\nThing 2: bar";

            Assert.AreEqual(expected, output);
        }

        public class ImageViewModel
        {
            public string src { get; set; }
        }



        [Test]
        public void HelperWithObjectArguments()
        {
        
            Handlebars.RegisterHelper("myHelper2", (writer, context, args) =>
            {
                var count = 0;
                foreach (var arg in args)
                {
                    ImageViewModel tempArg = (ImageViewModel)arg.ToType(typeof(ImageViewModel)) as ImageViewModel;
                    writer.Write("{0}", tempArg.src);
                }
            });

            var source = "Image was: {{myHelper2 image }}";

            var template = Handlebars.Compile(source);

            var output = template(new
            {
                image = new { src = "/url" }

            });

            var expected = "Image was: /url";

            Assert.AreEqual(expected, output);
        }

        [Test]
        public void InversionNoKey()
        {
            var source = "{{^key}}No key!{{/key}}";
            var template = Handlebars.Compile(source);
            var output = template(new { });
            var expected = "No key!";
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void InversionFalsy()
        {
            var source = "{{^key}}Falsy value!{{/key}}";
            var template = Handlebars.Compile(source);
            var data = new
            {
                key = false
            };
            var output = template(data);
            var expected = "Falsy value!";
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void InversionEmptySequence()
        {
            var source = "{{^key}}Empty sequence!{{/key}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    key = new string[] { }
                };
            var output = template(data);
            var expected = "Empty sequence!";
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void InversionNonEmptySequence()
        {
            var source = "{{^key}}Empty sequence!{{/key}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    key = new string[] { "element" }
                };
            var output = template(data);
            var expected = "";
            Assert.AreEqual(expected, output);
        }
    }

    public static class xx
    {
        public static object ToNonAnonymousList<T>(this List<T> list, Type t)
        {
            //define system Type representing List of objects of T type:
            Type genericType = typeof(List<>).MakeGenericType(t);

            //create an object instance of defined type:
            object l = Activator.CreateInstance(genericType);

            //get method Add from from the list:
            MethodInfo addMethod = l.GetType().GetMethod("Add");

            //loop through the calling list:
            foreach (T item in list)
            {
                //convert each object of the list into T object by calling extension ToType<T>()
                //Add this object to newly created list:
                addMethod.Invoke(l, new[] { item.ToType(t) });
            }
            //return List of T objects:
            return l;
        }
        public static object ToType<T>(this object obj, T type)
        {
            //create instance of T type object:
            object tmp = Activator.CreateInstance(Type.GetType(type.ToString()));

            //loop through the properties of the object you want to covert:          
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                try
                {
                    //get the value of property and try to assign it to the property of T type object:
                    tmp.GetType().GetProperty(pi.Name).SetValue(tmp, pi.GetValue(obj, null), null);
                }
                catch (Exception ex)
                {
                }
            }
            //return the T type object:         
            return tmp;
        }
    }
}

