using System;
using System.Collections.Generic;
using HandlebarsDotNet.PathStructure;
using Xunit;

namespace HandlebarsDotNet.Test
{
    // Data variables (`@index`, `@key`, `@first`, ...) are cached per frame as entry indexes into the
    // frame's data dictionary. These tests check that an unset data variable never resolves to the
    // value stored under a different key. See https://github.com/Handlebars-Net/Handlebars.Net/issues/707
    public class DataVariableTests
    {
        // Must match BindingContext's ContextDataObject: 16 buckets of 7 entries.
        private const int BucketsCount = 16;
        private const int BucketSize = 7;

        [Fact]
        public void UnsetDataVariableInChildFrameDoesNotResolveToAnotherDataKey()
        {
            var key = FindKeyForSlot(0);
            var data = new Dictionary<string, object> { [key] = "WRONG" };

            var template = Handlebars.Create().Compile("{{#with this}}[{{@index}}|{{@key}}|{{@first}}|{{@last}}]{{/with}}");

            Assert.Equal("[|||]", template(new { a = 1 }, data));
        }

        [Fact]
        public void UnsetDataVariableNeverResolvesToAnotherDataKeyAcrossPooledReuse()
        {
            var handlebars = Handlebars.Create();
            var populate = handlebars.Compile("{{#each this}}{{@index}}{{/each}}");
            var probe = handlebars.Compile("[{{@index}}]{{#with this}}[{{@index}}]{{/with}}");

            // `populate` stores @index at its home slot; put another key there (and at slot 0) for `probe`,
            // so a leftover index from an earlier render would point at a live entry.
            var data = new Dictionary<string, object>
            {
                [FindKeyForSlot(SlotOf(ChainSegment.Index.GetHashCode()))] = "WRONG",
                [FindKeyForSlot(0)] = "WRONG"
            };

            // Enough renders for the data dictionaries' byte versions to wrap several times.
            for (var i = 0; i < 2000; i++)
            {
                populate(new[] { 1, 2, 3 });
                Assert.Equal("[][]", probe(new { a = 1 }, data));
            }
        }

        private static string FindKeyForSlot(int slot)
        {
            for (var i = 0; ; i++)
            {
                var key = "k" + i;
                if (SlotOf(ChainSegment.Create(key).GetHashCode()) == slot) return key;
            }
        }

        private static int SlotOf(int hash) => (hash & (BucketsCount - 1)) * BucketSize + Math.Abs(hash % BucketSize);
    }
}
