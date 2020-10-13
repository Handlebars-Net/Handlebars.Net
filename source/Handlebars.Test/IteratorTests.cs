using System;
using System.Linq;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class IteratorTests
    {
        [Fact]
        public void BasicIterator()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{/each}}";
            var template = Handlebars.Create().Compile(source);
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
            Assert.Equal("Hello,\n- Erik\n- Helen", result);
        }

        [Fact]
        public void EmptyElementTemplate()
        {
            var source = "Hello,{{#each people}}{{/each}}";
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
            Assert.Equal("Hello,", result);
        }

        [Fact]
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
            Assert.Equal("Hello,\n0. Erik\n1. Helen", result);
        }

        [Fact]
        public void WithParentIndex()
        {
            var handlebars = Handlebars.Create();
            
            var source = @"
                {{#each level1}}
                    id={{id}}
                    index=[{{@../../index}}:{{@../index}}:{{@index}}]
                    first=[{{@../../first}}:{{@../first}}:{{@first}}]
                    last=[{{@../../last}}:{{@../last}}:{{@last}}]
                    {{#each level2}}
                        id={{id}}
                        index=[{{@../../index}}:{{@../index}}:{{@index}}]
                        first=[{{@../../first}}:{{@../first}}:{{@first}}]
                        last=[{{@../../last}}:{{@../last}}:{{@last}}]
                        {{#each level3}}
                            id={{id}}
                            index=[{{@../../index}}:{{@../index}}:{{@index}}]
                            first=[{{@../../first}}:{{@../first}}:{{@first}}]
                            last=[{{@../../last}}:{{@../last}}:{{@last}}]
                        {{/each}}
                    {{/each}}    
                {{/each}}";
            
            var template = handlebars.Compile( source );
            var data = new
                {
                    level1 = new[]{
                        new {
                            id = "0",
                            level2 = new[]{
                                new {
                                    id = "0-0",
                                    level3 = new[]{
                                        new { id = "0-0-0" },
                                        new { id = "0-0-1" }
                                    }
                                },
                                new {
                                    id = "0-1",
                                    level3 = new[]{
                                        new { id = "0-1-0" },
                                        new { id = "0-1-1" }
                                    }
                                }
                            }
                        },
                        new {
                            id = "1",
                            level2 = new[]{
                                new {
                                    id = "1-0",
                                    level3 = new[]{
                                        new { id = "1-0-0" },
                                        new { id = "1-0-1" }
                                    }
                                },
                                new {
                                    id = "1-1",
                                    level3 = new[]{
                                        new { id = "1-1-0" },
                                        new { id = "1-1-1" }
                                    }
                                }
                            }
                        }
                    }
            };

            var result = template( data );

            const string expected = @"
                            id=0
                            index=[::0]
                            first=[::True]
                            last=[::False]
                                id=0-0
                                index=[:0:0]
                                first=[:True:True]
                                last=[:False:False]
                                    id=0-0-0
                                    index=[0:0:0]
                                    first=[True:True:True]
                                    last=[False:False:False]
                                    id=0-0-1
                                    index=[0:0:1]
                                    first=[True:True:False]
                                    last=[False:False:True]
                                id=0-1
                                index=[:0:1]
                                first=[:True:False]
                                last=[:False:True]
                                    id=0-1-0
                                    index=[0:1:0]
                                    first=[True:False:True]
                                    last=[False:True:False]
                                    id=0-1-1
                                    index=[0:1:1]
                                    first=[True:False:False]
                                    last=[False:True:True]
                            id=1
                            index=[::1]
                            first=[::False]
                            last=[::True]
                                id=1-0
                                index=[:1:0]
                                first=[:False:True]
                                last=[:True:False]
                                    id=1-0-0
                                    index=[1:0:0]
                                    first=[False:True:True]
                                    last=[True:False:False]
                                    id=1-0-1
                                    index=[1:0:1]
                                    first=[False:True:False]
                                    last=[True:False:True]
                                id=1-1
                                index=[:1:1]
                                first=[:False:False]
                                last=[:True:True]
                                    id=1-1-0
                                    index=[1:1:0]
                                    first=[False:False:True]
                                    last=[True:True:False]
                                    id=1-1-1
                                    index=[1:1:1]
                                    first=[False:False:False]
                                    last=[True:True:True]";

            // Console.WriteLine(result); 

            Func<string, string> makeFlat = text => text.Replace( " ", "" ).Replace( "\n", "" ).Replace( "\r", "" );

            Assert.Equal( makeFlat( expected ), makeFlat( result ) );
        }

        [Fact]
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
            Assert.Equal("Hello,\n0. Erik (Erik is first)\n1. Helen (Helen is not first)", result);
        }

        [Fact]
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
            Assert.Equal("Hello,\n0. Erik (Erik is not last)\n1. Helen (Helen is last)", result);
        }

        [Fact]
        public void Empty()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{else}} (no one listed){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    people = new object[] { }
                };
            var result = template(data);
            Assert.Equal("Hello, (no one listed)", result);
        }

        [Fact]
        public void NullObject()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{else}} (no one listed){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    people = (object)null
                };
            var result = template(data);
            Assert.Equal("Hello, (no one listed)", result);
        }

        [Fact]
        public void NullSequence()
        {
            var source = "Hello,{{#each people}}\n- {{name}}{{else}} (no one listed){{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
                {
                    people = (object[])null
                };
            var result = template(data);
            Assert.Equal("Hello, (no one listed)", result);
        }
        
        [Fact]
        public void EnumerableIterator()
        {
            var source = "{{#each people}}{{.}}{{@index}}{{/each}}";
            var template = Handlebars.Compile(source);
            var data = new
            {
                people = Enumerable.Range(0, 3).Select(x => x.ToString())
            };
            var result = template(data);
            Assert.Equal("001122", result);
        }
    }
}

