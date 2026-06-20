using System.Collections.Generic;
using Xunit;

namespace HandlebarsDotNet.Test
{
    // Issue https://github.com/Handlebars-Net/Handlebars.Net/issues/595
    // NotSupportedException: TypeConverter cannot convert UndefinedBindingResult
    // when using `{{#with this as |field|}}` inside `{{#each Fields}}` and then
    // passing `field` as an argument to a helper in an inner `{{#each (helper field)}}`.
    public class Issue595Tests
    {
        [Fact]
        public void BlockParamFromWithShouldBePassableToHelperInInnerEach()
        {
            var handlebars = Handlebars.Create();
            var receivedArgs = new List<object>();

            handlebars.RegisterHelper("Getattributes", (context, arguments) =>
            {
                receivedArgs.Add(arguments[0]);
                return new[] { "attr1", "attr2" };
            });

            var template = handlebars.Compile(
                "{{#each Fields}}" +
                "{{#with this as |field|}}" +
                "{{#each (Getattributes field)}}" +
                "{{this}} " +
                "{{/each}}" +
                "{{/with}}" +
                "{{/each}}"
            );

            var data = new
            {
                Fields = new[] { "field1", "field2" }
            };

            // Should not throw NotSupportedException: TypeConverter cannot convert UndefinedBindingResult
            var result = template(data);

            Assert.Equal("attr1 attr2 attr1 attr2 ", result);

            // Verify that the helper received the actual field values, not UndefinedBindingResult
            Assert.Equal(2, receivedArgs.Count);
            Assert.Equal("field1", receivedArgs[0]);
            Assert.Equal("field2", receivedArgs[1]);
        }

        [Fact]
        public void BlockParamTypedAccessShouldNotThrowWhenPassedToHelper()
        {
            // Reproduces the exact error:
            // System.NotSupportedException: TypeConverter cannot convert UndefinedBindingResult to string
            // The bug manifests when field resolves to UndefinedBindingResult and the helper
            // uses arguments.At<T>() (typed access) which calls TypeConverter.ConvertTo.
            var handlebars = Handlebars.Create();
            var receivedArgs = new List<string>();

            handlebars.RegisterHelper("Getattributes", (context, arguments) =>
            {
                // Using At<T>() typed access triggers the NotSupportedException if
                // arguments[0] is UndefinedBindingResult rather than the actual field value
                var fieldValue = arguments.At<string>(0);
                receivedArgs.Add(fieldValue);
                return new[] { "attr1", "attr2" };
            });

            var template = handlebars.Compile(
                "{{#each Fields}}" +
                "{{#with this as |field|}}" +
                "{{#each (Getattributes field)}}" +
                "{{this}} " +
                "{{/each}}" +
                "{{/with}}" +
                "{{/each}}"
            );

            var data = new
            {
                Fields = new[] { "field1", "field2" }
            };

            // Should not throw NotSupportedException: TypeConverter cannot convert UndefinedBindingResult to string
            var result = template(data);

            Assert.Equal("attr1 attr2 attr1 attr2 ", result);
            Assert.Equal(2, receivedArgs.Count);
            Assert.Equal("field1", receivedArgs[0]);
            Assert.Equal("field2", receivedArgs[1]);
        }

        [Fact]
        public void BlockParamFromWithShouldBePassableToHelperInInnerEachMultipleIterations()
        {
            // This test uses more iterations to increase the chance of pool reuse,
            // which is what triggers the stale-data bug.
            var handlebars = Handlebars.Create();
            var receivedArgs = new List<string>();

            handlebars.RegisterHelper("Getattributes", (context, arguments) =>
            {
                var fieldValue = arguments.At<string>(0);
                receivedArgs.Add(fieldValue);
                return new[] { "x", "y" };
            });

            var template = handlebars.Compile(
                "{{#each Fields}}" +
                "{{#with this as |field|}}" +
                "{{#each (Getattributes field)}}" +
                "{{this}}" +
                "{{/each}}" +
                "{{/with}}" +
                "{{/each}}"
            );

            var data = new
            {
                Fields = new[] { "a", "b", "c", "d", "e" }
            };

            var result = template(data);

            Assert.Equal("xyxyxyxyxy", result);
            Assert.Equal(5, receivedArgs.Count);
            Assert.Equal(new[] { "a", "b", "c", "d", "e" }, receivedArgs);
        }

        [Fact]
        public void Issue595_WithBlockParamsInEach_DoesNotThrow()
        {
            // Canonical 1000-iteration stress test to exercise BindingContext pool reuse.
            // If the block param `field` ever resolves to UndefinedBindingResult due to
            // pool reuse corruption, the helper would receive a wrong value (or throw
            // NotSupportedException when typed access is used).
            var h = Handlebars.Create();
            h.RegisterHelper("Getattributes", (context, args) => new[] { "attr1", "attr2" });

            var template = h.Compile(
                "{{#each Fields}}" +
                "{{#with this as |field|}}" +
                "{{#each (Getattributes field)}}" +
                "{{this}} " +
                "{{/each}}" +
                "{{/with}}" +
                "{{/each}}");

            var data = new { Fields = new object[] { new { Name = "f1" }, new { Name = "f2" } } };

            // Run many times to stress pool reuse
            for (int i = 0; i < 1000; i++)
            {
                var result = template(data);
                Assert.Contains("attr1", result);
                Assert.Contains("attr2", result);
            }
        }
    }
}
