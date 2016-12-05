using System;
using NUnit.Framework;

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
		public void WithParentIndex()
		{
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
			var template = Handlebars.Compile( source );
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

			Func<string, string> makeFlat = text => text.Replace( "\t", "" ).Replace( "\n", "" ).Replace( "\r", "" );

			Assert.AreEqual( makeFlat( expected ), makeFlat( result ) );
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

