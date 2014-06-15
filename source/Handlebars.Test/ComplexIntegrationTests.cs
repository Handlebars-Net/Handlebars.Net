using NUnit.Framework;
using System;
using System.IO;
using System.Collections;

namespace Handlebars.Test
{
    [TestFixture]
    public class ComplexIntegrationTests
    {
        [Test]
        public void DeepIf()
        {
            var source = 
@"{{#if outer_bool}}
{{#with a}}{{#if inner_bool}}a is true{{else}}a is false{{/if}}{{/with}}
{{else}}
{{#with b}}{{#if inner_bool}}b is true{{else}}b is false{{/if}}{{/with}}
{{/if}}";
            var template = Handlebars.Compile(source);
            var trueTrue = new {
                outer_bool = true,
                a = new {
                    inner_bool = true
                }
            };
            var trueFalse = new {
                outer_bool = true,
                a = new {
                    inner_bool = false
                }
            };
            var falseTrue = new {
                outer_bool = false,
                b = new {
                    inner_bool = true
                }
            };
            var falseFalse = new {
                outer_bool = false,
                b = new {
                    inner_bool = false
                }
            };
            var resultTrueTrue = template(trueTrue);
            var resultTrueFalse = template(trueFalse);
            var resultFalseTrue = template(falseTrue);
            var resultFalseFalse = template(falseFalse);
            Assert.AreEqual("\na is true\n", resultTrueTrue);
            Assert.AreEqual("\na is false\n", resultTrueFalse);
            Assert.AreEqual("\nb is true\n", resultFalseTrue);
            Assert.AreEqual("\nb is false\n", resultFalseFalse);
        }

        [Test]
        public void IfImplicitIteratorHelper()
        {
            var source = "{{#if outer_bool}}{{#items}}{{link_to url text}}{{/items}}{{/if}}";

            var template = Handlebars.Compile(source);

            var data = new {
                outer_bool = true,
                items = new [] 
                {
                    new { text = "Google", url = "http://google.com/" },
                    new { text = "Yahoo!", url = "http://yahoo.com/" }
                }
            };

            Handlebars.RegisterHelper("link_to", (writer, context, parameters) => {
                writer.WriteSafeString("<a href='" + parameters[0] + "'>" + parameters[1] + "</a>");
            });

            var result = template(data);
            Assert.AreEqual("<a href='http://google.com/'>Google</a><a href='http://yahoo.com/'>Yahoo!</a>", result);
        }

        //[Test]
        public void BlockHelperHelper()
        {
            var source = "{{#block_helper foo}}{{link_to url text}}{{/block_helper}}";
            
            var template = Handlebars.Compile(source);

            var data = new {
                foo = new [] 
                {
                    new { text = "Google", url = "http://google.com/" },
                    new { text = "Yahoo!", url = "http://yahoo.com/" }
                }
            };

            Handlebars.RegisterHelper("link_to", (writer, context, parameters) => {
                writer.WriteSafeString("<a href='" + parameters[0] + "'>" + parameters[1] + "</a>");
            });

            Handlebars.RegisterHelper("block_helper", (writer, blockTemplate, context, arguments) => {
                foreach(var item in arguments[0] as IEnumerable)
                {
                    blockTemplate(writer, item);
                }
            });

            var result = template(data);
            Assert.AreEqual("<a href='http://google.com/'>Google</a><a href='http://yahoo.com/'>Yahoo!</a>", result);
        }
    }
}

