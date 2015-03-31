using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace HandlebarsDotNet.Test
{
    [TestFixture]
    public class IteratorTests
    {
        [Test]
        public void BasicIterator()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{/each}}";
            var template = Handlebars.Compile(source);
            var data = new {
                people = new []{
                    new { 
                        name = "Erik"
                    },
                    new {
                        name = "Helen"
                    }
                }
            };
            var result = template(data);
            Assert.AreEqual("Hello,\n- Erik\n- Helen", result);
        }

        [Test]
        public void WithIndex()
        {
            var source = "Hello,{{#each people}}\n{{@index}}. {{name}}{{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    people = new[]{
                        new { 
                            name = "Erik"
                        },
                        new {
                            name = "Helen"
                        }
                    }
                };
            var result = template(data);
            Assert.AreEqual("Hello,\n0. Erik\n1. Helen", result);
        }

        [Test]
        public void WithFirst()
        {
            var source = "Hello,{{#each people}}\n{{@index}}. {{name}} ({{name}} is {{#if @first}}first{{else}}not first{{/if}}){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    people = new[]{
                        new { 
                            name = "Erik"
                        },
                        new {
                            name = "Helen"
                        }
                    }
                };
            var result = template(data);
            Assert.AreEqual("Hello,\n0. Erik (Erik is first)\n1. Helen (Helen is not first)", result);
        }

        [Test]
        public void WithLast()
        {
            var source = "Hello,{{#each people}}\n{{@index}}. {{name}} ({{name}} is {{#if @last}}last{{else}}not last{{/if}}){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    people = new[]{
                        new { 
                            name = "Erik"
                        },
                        new {
                            name = "Helen"
                        }
                    }
                };
            var result = template(data);
            Assert.AreEqual("Hello,\n0. Erik (Erik is not last)\n1. Helen (Helen is last)", result);
        }

        [Test]
        public void Empty()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{else}} (no one listed){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    people = new object[] { }
                };
            var result = template(data);
            Assert.AreEqual("Hello, (no one listed)", result);
        }

        [Test]
        public void NullObject()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{else}} (no one listed){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    people = (object)null
                };
            var result = template(data);
            Assert.AreEqual("Hello, (no one listed)", result);
        }

        [Test]
        public void NullSequence()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{else}} (no one listed){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    people = (object[])null
                };
            var result = template(data);
            Assert.AreEqual("Hello, (no one listed)", result);
        }
    }
}

