using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;

namespace HandlebarsNet.Benchmark
{
    // Render time for the string-returning template API — the most common way the
    // library is consumed. Unlike the other Render* benchmarks (which write to
    // TextWriter.Null and therefore only measure resolution overhead), this suite
    // pays the real output cost: HTML encoding and StringBuilder-backed writes.
    //
    // Content=clean:  values contain nothing that needs HTML escaping (typical data).
    // Content=html:   values are dense with characters that must be escaped (worst case).
    [MemoryDiagnoser]
    public class RenderToString
    {
        private HandlebarsTemplate<object, object> _template;
        private object _data;

        private const int ItemCount = 50;

        private const string Source =
            "<ul>" +
            "{{#each items}}" +
            "<li id=\"item-{{@index}}\"><span>{{name}}</span> &mdash; {{description}} " +
            "<em>x{{qty}}</em>{{#if onSale}} <strong>SALE</strong>{{/if}}</li>" +
            "{{/each}}" +
            "</ul>";

        [Params("clean", "html")]
        public string Content { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var handlebars = Handlebars.Create();
            _template = handlebars.Compile(Source);

            var description = Content == "clean"
                ? "Reliable everyday item for home and office use"
                : "Fast & reliable <everyday> item \"for\" home & office use";

            var items = new List<object>(ItemCount);
            for (var i = 0; i < ItemCount; i++)
            {
                items.Add(new
                {
                    name = $"Product {i:D4}",
                    description,
                    qty = i * 7 + 1,
                    onSale = i % 3 == 0
                });
            }

            _data = new { items };
        }

        [Benchmark]
        public string Render() => _template(_data);
    }
}
