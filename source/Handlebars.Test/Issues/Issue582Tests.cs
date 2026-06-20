using System.Collections.Generic;
using Xunit;

namespace HandlebarsDotNet.Test
{
    public class Issue582Tests
    {
        private static IHandlebars CreateHandlebars()
        {
            var config = new HandlebarsConfiguration
            {
                ExpressionNameResolver = new HandlebarsDotNet.Compiler.Resolvers.UpperCamelCaseExpressionNameResolver()
            };
            return Handlebars.Create(config);
        }

        [Fact]
        public void Issue582_UpperCamelCaseResolverDoesNotBreakEachIteration()
        {
            var h = CreateHandlebars();
            var template = h.Compile("{{#each items}}{{name}} {{/each}}");
            var data = new { items = new[] { new { name = "Alice" }, new { name = "Bob" } } };
            Assert.Equal("Alice Bob ", template(data));
        }

        [Fact]
        public void Issue582_UpperCamelCaseResolverDoesNotBreakEachWithList()
        {
            var h = CreateHandlebars();
            var template = h.Compile("{{#each items}}{{name}} {{/each}}");
            var data = new
            {
                items = new List<object>
                {
                    new { name = "Alice" },
                    new { name = "Bob" }
                }
            };
            Assert.Equal("Alice Bob ", template(data));
        }

        [Fact]
        public void Issue582_UpperCamelCaseResolverDoesNotBreakEachWithAtIndex()
        {
            var h = CreateHandlebars();
            var template = h.Compile("{{#each items}}{{@index}}:{{name}} {{/each}}");
            var data = new { items = new[] { new { name = "Alice" }, new { name = "Bob" } } };
            Assert.Equal("0:Alice 1:Bob ", template(data));
        }

        [Fact]
        public void Issue582_UpperCamelCaseResolverDoesNotBreakEachWithAtFirst()
        {
            var h = CreateHandlebars();
            var template = h.Compile("{{#each items}}{{#if @first}}first:{{/if}}{{name}} {{/each}}");
            var data = new { items = new[] { new { name = "Alice" }, new { name = "Bob" } } };
            Assert.Equal("first:Alice Bob ", template(data));
        }

        [Fact]
        public void Issue582_UpperCamelCaseResolverWorksWithNestedPropertyAccess()
        {
            var h = CreateHandlebars();
            var template = h.Compile("{{#each items}}{{address.city}} {{/each}}");
            var data = new
            {
                items = new[]
                {
                    new { address = new { city = "New York" } },
                    new { address = new { city = "London" } }
                }
            };
            Assert.Equal("New York London ", template(data));
        }

        [Fact]
        public void Issue582_UpperCamelCaseResolverWorksWithStringArray()
        {
            var h = CreateHandlebars();
            var template = h.Compile("{{#each items}}{{this}} {{/each}}");
            var data = new { items = new[] { "Alice", "Bob" } };
            Assert.Equal("Alice Bob ", template(data));
        }

        [Fact]
        public void Issue582_UpperCamelCaseResolverWorksWithNestedEach()
        {
            var h = CreateHandlebars();
            var template = h.Compile("{{#each groups}}{{#each members}}{{name}} {{/each}}{{/each}}");
            var data = new
            {
                groups = new[]
                {
                    new { members = new[] { new { name = "Alice" }, new { name = "Bob" } } },
                    new { members = new[] { new { name = "Carol" } } }
                }
            };
            Assert.Equal("Alice Bob Carol ", template(data));
        }
    }
}
