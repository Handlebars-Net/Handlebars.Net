using Xunit;

namespace HandlebarsDotNet.Test
{
    public class InlinePartialTests
    {
        [Fact]
        public void BasicInlinePartial()
        {
            string source = "{{#*inline \"person\"}}{{name}}{{/inline}}Hello, {{>person}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                name = "Marc"
            };

            var result = template(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicInlinePartialWithNameConflicts()
        {
            string source = "{{#*inline \"personConflict\"}}{{firstName}}{{/inline}}Hello, {{>personConflict}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                firstName = "Marc",
                lastName = "Jones"
            };

            var result = template(data);
            Assert.Equal("Hello, Marc!", result);

            string source2 = "{{#*inline \"personConflict\"}}{{firstName}} {{lastName}}{{/inline}}Hello, {{>personConflict}}!";

            var template2 = Handlebars.Compile(source2);

            var result2 = template2(data);

            Assert.Equal("Hello, Marc Jones!", result2);

            var rerunFirstResult = template(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicStringOnlyInlinePartial()
        {
            string source = "{{#*inline \"person\"}}{{name}}{{/inline}}Hello, {{>person}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                name = "Marc"
            };

            var result = template(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicInlinePartialWithContext()
        {
            string source = "{{#*inline \"person\"}}{{name}}{{/inline}}Hello, {{>person leadDev}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                leadDev = new
                {
                    name = "Marc"
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicInlinePartialWithStringParameter()
        {
            string source = "{{#*inline \"person\"}}{{first}}{{/inline}}Hello, {{>person first='Pete'}}!";

            var template = Handlebars.Compile(source);

            var result = template(null);
            Assert.Equal("Hello, Pete!", result);
        }

        [Fact]
        public void BasicInlinePartialWithMultipleStringParameters()
        {
            string source = "{{#*inline \"person\"}}{{first}} {{last}}{{/inline}}Hello, {{>person first='Pete' last=\"Sampras\"}}!";

            var template = Handlebars.Compile(source);

            var result = template(null);
            Assert.Equal("Hello, Pete Sampras!", result);
        }

        [Fact]
        public void BasicInlinePartialWithContextParameter()
        {
            string source = "{{#*inline \"person\"}}{{first.name}}{{/inline}}Hello, {{>person first=leadDev.marc}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                leadDev = new
                {
                    marc = new
                    {
                        name = "Marc"
                    }
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicInlinePartialWithContextAndStringParameters()
        {
            string source = "{{#*inline \"person\"}}{{first.name}} {{last}}{{/inline}}Hello, {{>person first=leadDev.marc last='Smith'}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                leadDev = new
                {
                    marc = new
                    {
                        name = "Marc"
                    }
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Marc Smith!", result);
        }

        [Fact]
        public void BasicInlinePartialWithTypedParameters()
        {
            string source = "{{#*inline \"person\"}}{{first}} {{last}}{{/inline}}Hello, {{>person first=1 last=true}}!";

            var template = Handlebars.Compile(source);

            var result = template(null);
            Assert.Equal("Hello, 1 True!", result);
        }

        [Fact]
        public void BasicInlinePartialWithStringParameterIncludingExpressionChars()
        {
            string source = "{{#*inline \"person\"}}{{first}}{{/inline}}Hello, {{>person first='Pe ({~te~}) '}}!";

            var template = Handlebars.Compile(source);

            var result = template(null);
            Assert.Equal("Hello, Pe ({~te~}) !", result);
        }

        [Fact]
        public void SuperfluousWhitespace()
        {
            string source = "{{#*inline \"person\"}}{{name}}{{/inline}}Hello, {{  >  person  }}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                name = "Marc"
            };

            var result = template(data);
            Assert.Equal("Hello, Marc!", result);
        }

        [Fact]
        public void BasicInlinePartialWithStringParametersAndImplicitContext()
        {
            string source = "{{#*inline \"person\"}}{{firstName}} {{lastName}}{{/inline}}Hello, {{>person lastName='Smith'}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                firstName = "Marc",
                lastName = "Jones"
            };

            var result = template(data);
            Assert.Equal("Hello, Marc Smith!", result);
        }

        [Fact]
        public void BasicInlinePartialWithEmptyParameterDoesNotFallback()
        {
            string source = "{{#*inline \"person\"}}{{firstName}} {{lastName}}{{/inline}}Hello, {{>person lastName=test}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                firstName = "Marc",
                lastName = "Jones"
            };

            var result = template(data);
            Assert.Equal("Hello, Marc !", result);
        }

        [Fact]
        public void BasicInlinePartialWithIncompleteChildContextDoesNotFallback()
        {
            string source = "{{#*inline \"person\"}}{{firstName}} {{lastName}}{{/inline}}Hello, {{>person leadDev}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                firstName = "Pete",
                lastName = "Jones",
                leadDev = new
                {
                    firstName = "Marc"
                }
            };

            var result = template(data);
            Assert.Equal("Hello, Marc !", result);
        }

        [Fact]
        public void BasicBlockInlinePartial()
        {
            string source = "Hello, {{#>personInline}}friend{{/personInline}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                firstName = "Pete",
                lastName = "Jones"
            };

            var result1 = template(data);
            Assert.Equal("Hello, friend!", result1);

            source = "{{#*inline \"personInline\"}}{{firstName}} {{lastName}}{{/inline}}" + source;
            template = Handlebars.Compile(source);

            var result2 = template(data);
            Assert.Equal("Hello, Pete Jones!", result2);
        }

        [Fact]
        public void BasicBlockInlinePartialWithArgument()
        {
            string source = "Hello, {{#>personInline arg='Todd'}}friend{{/personInline}}!";

            var template = Handlebars.Compile(source);

            var data = new
            {
                firstName = "Pete",
                lastName = "Jones"
            };

            var result1 = template(data);
            Assert.Equal("Hello, friend!", result1);

            source = "{{#*inline \"personInline\"}}{{arg}}{{/inline}}" + source;
            template = Handlebars.Compile(source);

            var result2 = template(data);
            Assert.Equal("Hello, Todd!", result2);
        }

        [Fact]
        public void BlockinlinePartialWithSpecialNamedPartial()
        {
            string source = "{{#*inline \"myPartial\"}}this is {{> @partial-block }} content{{/inline}}Well, {{#>myPartial}}some test{{/myPartial}} !";

            var template = Handlebars.Compile(source);

            var data = new { };
            var result = template(data);

            Assert.Equal("Well, this is some test content !", result);
        }

        [Fact]
        public void BlockInlinePartialWithNestedSpecialNamedPartial()
        {
            string source = "{{#*inline \"partial2\"}}that {{> @partial-block}} great {{firstName}}{{/inline}}" 
                          + "{{#*inline \"partial1\"}}this is {{> @partial-block }} content {{#>partial2}}works{{/partial2}} {{lastName}}{{/inline}}"
                          + "Well, {{#>partial1}}some test{{/partial1}} !";


            var template = Handlebars.Compile(source);

            //var partialSource1 = "this is {{> @partial-block }} content {{#>partial2}}works{{/partial2}} {{lastName}}";
            //using (var reader = new StringReader(partialSource1))
            //{
            //    var partialTemplate = Handlebars.Compile(reader);
            //    Handlebars.RegisterTemplate("partial1", partialTemplate);
            //}

            //var partialSource2 = "that {{> @partial-block}} great {{firstName}}";
            //using (var reader = new StringReader(partialSource2))
            //{
            //    var partialTemplate = Handlebars.Compile(reader);
            //    Handlebars.RegisterTemplate("partial2", partialTemplate);
            //}

            var data = new
            {
                firstName = "Pete",
                lastName = "Jones"
            };
            var result = template(data);

            Assert.Equal("Well, this is some test content that works great Pete Jones !", result);
        }

        [Fact]
        public void InlinePartialInEach()
        {
            string source = "{{#*inline \"item\"}}{{id}}{{/inline}}{{#each items}}{{>item}}{{/each}}";

            var template = Handlebars.Compile(source);

            var data = new
            {
                items = new[]
                {
                    new
                    { id = 1 },
                    new { id = 2 }
                }
            };

            var result = template(data);
            Assert.Equal("12", result);
        }
    }
}

