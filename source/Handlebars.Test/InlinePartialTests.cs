using System.Collections.Generic;
using HandlebarsDotNet.Compiler;
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
        public void InlinePartialDefinitionWithExtraArgumentDoesNotThrow()
        {
            // Regression test for https://github.com/Handlebars-Net/Handlebars.Net/issues/560
            string source = "{{#*inline \"my_partial\" arg}}{{arg}}{{/inline}}{{>my_partial}}";

            var template = Handlebars.Compile(source);
            var result = template(null);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void InlinePartialDefinitionWithHashArgumentDoesNotThrow()
        {
            string source = "{{#*inline \"my_partial\" arg=5}}{{arg}}{{/inline}}{{>my_partial}}";

            var template = Handlebars.Compile(source);
            var result = template(null);

            Assert.Equal(string.Empty, result);
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
            
            source = "Hello, {{#>personInline}}{{/personInline}}!";
            template = Handlebars.Compile(source);

            var result3 = template(data);
            Assert.Equal("Hello, !", result3);

            source = "{{#*inline \"personInline\"}}{{firstName}} {{lastName}}{{/inline}}" + source;
            template = Handlebars.Compile(source);

            var result4 = template(data);
            Assert.Equal("Hello, Pete Jones!", result4);
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

        [Fact]
        public void RecursionUnboundedInlinePartial()
        {
            string source = "{{#*inline \"list\"}}{{>list}}{{/inline}}{{>list}}";

            var template = Handlebars.Compile(source);

            string Result() => template(null);
            var ex = Assert.Throws<HandlebarsRuntimeException>(Result);
            while (ex.InnerException != null)
                ex = Assert.IsType<HandlebarsRuntimeException>(ex.InnerException);
            Assert.Equal("Runtime error while rendering partial 'list', exceeded recursion depth limit of 100", ex.Message);
        }

        [Fact]
        public void RecursionBoundedToLimitInlinePartial()
        {
            string source = "{{#*inline \"list\"}}x{{#each items}}{{#if items}}{{>list}}{{/if}}{{/each}}{{/inline}}{{>list}}{{>list}}";

            var template = Handlebars.Compile(source);

            var data = new Dictionary<string, object>();
            var items = data;
            for (var depth = 0; depth < Handlebars.Configuration.PartialRecursionDepthLimit; depth++)
            {
                var nestedItems = new Dictionary<string, object>();
                items.Add("items", new[] { nestedItems });
                items = nestedItems;
            }

            var result = template(data);
            Assert.Equal(new string('x', Handlebars.Configuration.PartialRecursionDepthLimit * 2), result);
        }

        [Fact]
        public void RecursionBoundedAboveLimitInlinePartial()
        {
            string source = "{{#*inline \"list\"}}x{{#each items}}{{#if items}}{{>list}}{{/if}}{{/each}}{{/inline}}{{>list}}";

            var template = Handlebars.Compile(source);

            var data = new Dictionary<string, object>();
            var items = data;
            for (var depth = 0; depth < Handlebars.Configuration.PartialRecursionDepthLimit + 1; depth++)
            {
                var nestedItems = new Dictionary<string, object>();
                items.Add("items", new[] { nestedItems });
                items = nestedItems;
            }

            string Result() => template(data);
            var ex = Assert.Throws<HandlebarsRuntimeException>(Result);
            while (ex.InnerException != null)
                ex = Assert.IsType<HandlebarsRuntimeException>(ex.InnerException);
            Assert.Equal("Runtime error while rendering partial 'list', exceeded recursion depth limit of 100", ex.Message);
        }

        [Fact]
        public void BlockInlinePartialWithInlinePartials()
        {
            string partialSource = "{{#*inline \"greeting\"}}{{#>salutation}}Dear{{/salutation}} {{#>name}}{{firstName}} {{lastName}}{{/name}}{{/inline}}";

            var data = new
            {
                firstName = "Pete",
                lastName = "Jones"
            };

            string source = partialSource + "{{#>greeting}}{{/greeting}}";
            var template = Handlebars.Compile(source);

            var result1 = template(data);
            Assert.Equal("Dear Pete Jones", result1);

            source = partialSource + "{{#>greeting}}{{#*inline \"salutation\"}}Hello{{/inline}}{{/greeting}}";
            template = Handlebars.Compile(source);

            var result2 = template(data);
            Assert.Equal("Hello Pete Jones", result2);
            
            source = partialSource + "{{#>greeting}}{{#*inline \"name\"}}Mr. {{lastName}}{{/inline}}{{/greeting}}";
            template = Handlebars.Compile(source);

            var result3 = template(data);
            Assert.Equal("Dear Mr. Jones", result3);

            source = partialSource + "{{#>greeting}}{{#*inline \"salutation\"}}Hello{{/inline}}{{#*inline \"name\"}}{{firstName}}{{/inline}}{{/greeting}}";
            template = Handlebars.Compile(source);

            var result4 = template(data);
            Assert.Equal("Hello Pete", result4);
        }

        // https://github.com/Handlebars-Net/Handlebars.Net/issues/673
        // handlebars.js spec/partials.js: "should define inline partials for block"
        [Fact]
        public void InlinePartialIsScopedToDeclaringBlock()
        {
            var handlebars = Handlebars.Create();

            var inside = handlebars.Compile("{{#with .}}{{#*inline \"myPartial\"}}success{{/inline}}{{> myPartial}}{{/with}}");
            Assert.Equal("success", inside(new { }));

            var outside = handlebars.Compile("{{#with .}}{{#*inline \"myPartial\"}}success{{/inline}}{{/with}}{{> myPartial}}");
            var exception = Assert.Throws<HandlebarsRuntimeException>(() => outside(new { }));
            Assert.Contains("'myPartial' could not be resolved", exception.Message);
        }

        [Fact]
        public void InlinePartialInBlockOverridesOuterOnlyInsideBlock()
        {
            var handlebars = Handlebars.Create();
            var template = handlebars.Compile(
                "{{#*inline \"myPartial\"}}outer{{/inline}}" +
                "{{#with .}}{{#*inline \"myPartial\"}}inner{{/inline}}{{> myPartial}}{{/with}} " +
                "{{> myPartial}}");

            Assert.Equal("inner outer", template(new { }));
        }

        [Fact]
        public void InlinePartialInBlockIsVisibleToNestedBlocks()
        {
            var handlebars = Handlebars.Create();
            var template = handlebars.Compile(
                "{{#with .}}{{#*inline \"myPartial\"}}success{{/inline}}" +
                "{{#with .}}{{#if true}}{{> myPartial}}{{/if}}{{/with}}{{/with}}");

            Assert.Equal("success", template(new { }));
        }

        [Fact]
        public void InlinePartialInEachIsScopedToLoop()
        {
            var handlebars = Handlebars.Create();
            var template = handlebars.Compile(
                "{{#*inline \"item\"}}outer{{/inline}}" +
                "{{#each items}}{{#*inline \"item\"}}[{{.}}]{{/inline}}{{> item}}{{/each}} {{> item}}");

            Assert.Equal("[1][2] outer", template(new { items = new[] { 1, 2 } }));
        }

        [Fact]
        public void LastInlinePartialDefinitionInBlockWins()
        {
            var handlebars = Handlebars.Create();
            var template = handlebars.Compile(
                "{{#with .}}{{#*inline \"p\"}}first{{/inline}}{{#*inline \"p\"}}second{{/inline}}{{> p}}{{/with}}");

            Assert.Equal("second", template(new { }));
        }

        [Fact]
        public void InlinePartialsInBlockWithCustomDecoratorAreScopedToBlock()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterDecorator("noop",
                (TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) => function);

            var template = handlebars.Compile(
                "{{#with .}}{{#*inline \"a\"}}A{{/inline}}{{*noop}}{{#*inline \"b\"}}B{{/inline}}{{> a}}{{> b}}{{/with}}{{> a}}");

            var exception = Assert.Throws<HandlebarsRuntimeException>(() => template(new { }));
            Assert.Contains("'a' could not be resolved", exception.Message);

            var inside = handlebars.Compile(
                "{{#with .}}{{#*inline \"a\"}}A{{/inline}}{{*noop}}{{#*inline \"b\"}}B{{/inline}}{{> a}}{{> b}}{{/with}}");
            Assert.Equal("AB", inside(new { }));
        }

        [Fact]
        public void InlinePartialInPartialBlockIsScopedToPartialBlock()
        {
            var handlebars = Handlebars.Create();
            handlebars.RegisterTemplate("layout", "<{{> content}}>");

            var template = handlebars.Compile(
                "{{#> layout}}{{#*inline \"content\"}}inside{{/inline}}{{/layout}}");
            Assert.Equal("<inside>", template(new { }));

            var after = handlebars.Compile(
                "{{#> layout}}{{#*inline \"content\"}}inside{{/inline}}{{/layout}}{{> content}}");
            var exception = Assert.Throws<HandlebarsRuntimeException>(() => after(new { }));
            Assert.Contains("'content' could not be resolved", exception.Message);
        }

        [Fact]
        public void InlinePartialScopeIsRestoredOnRepeatedRenders()
        {
            var handlebars = Handlebars.Create();
            var template = handlebars.Compile(
                "{{#*inline \"p\"}}outer{{/inline}}{{#with .}}{{#*inline \"p\"}}inner{{/inline}}{{> p}}{{/with}}{{> p}}");

            Assert.Equal("innerouter", template(new { }));
            Assert.Equal("innerouter", template(new { }));
        }
    }
}

