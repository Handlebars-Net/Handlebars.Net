using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;

namespace HandlebarsNet.Benchmark
{
    // Render time for a flat list template: one {{#each}} loop over N items,
    // each item has multiple property accesses and two conditional blocks.
    // Primary scaling axis for regression detection.
    [MemoryDiagnoser]
    public class RenderList
    {
        private HandlebarsTemplate<TextWriter, object, object> _template = null!; // -> Setup()
        private object _data = null!; // -> Setup()

        private const string Source =
            "{{#each items}}" +
            "{{name}} | {{category}} | ${{price}}" +
            "{{#if onSale}} [SALE ${{salePrice}}]{{/if}}" +
            " | {{#if inStock}}In Stock{{else}}Sold Out{{/if}}" +
            " | SKU: {{sku}}\n" +
            "{{/each}}";

        [Params(10, 100, 1000)]
        public int N { get; set; }

        [Params("object", "dictionary")]
        public string DataType { get; set; } = null!;

        [GlobalSetup]
        public void Setup()
        {
            var handlebars = Handlebars.Create();
            _template = handlebars.Compile(new StringReader(Source));

            _data = DataType switch
            {
                "object"     => new { items = BuildObjectItems() },
                _            => new Dictionary<string, object> { ["items"] = BuildDictionaryItems() }
            };
        }

        [Benchmark]
        public void Render() => _template(TextWriter.Null, _data);

        private static readonly string[] Categories = ["Electronics", "Books", "Clothing", "Home & Garden", "Sports"];

        private List<object> BuildObjectItems()
        {
            var items = new List<object>(N);
            for (int i = 0; i < N; i++)
            {
                items.Add(new
                {
                    name     = $"Product {i:D4}",
                    category = Categories[i % Categories.Length],
                    price    = $"{9.99 + i * 1.49:F2}",
                    onSale   = i % 3 == 0,
                    salePrice = $"{4.99 + i * 0.99:F2}",
                    sku      = $"SKU-{i:D6}",
                    inStock  = i % 7 != 0
                });
            }
            return items;
        }

        private List<Dictionary<string, object>> BuildDictionaryItems()
        {
            var items = new List<Dictionary<string, object>>(N);
            for (int i = 0; i < N; i++)
            {
                items.Add(new Dictionary<string, object>
                {
                    ["name"]      = $"Product {i:D4}",
                    ["category"]  = Categories[i % Categories.Length],
                    ["price"]     = $"{9.99 + i * 1.49:F2}",
                    ["onSale"]    = i % 3 == 0,
                    ["salePrice"] = $"{4.99 + i * 0.99:F2}",
                    ["sku"]       = $"SKU-{i:D6}",
                    ["inStock"]   = i % 7 != 0
                });
            }
            return items;
        }
    }
}
