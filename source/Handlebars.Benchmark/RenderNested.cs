using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;

namespace HandlebarsNet.Benchmark
{
    // Render time for a nested-loop template: a fixed number of sections each
    // containing RowsPerSection metric rows. Tests context-push/pop overhead,
    // nested property resolution, and per-row conditionals at scale.
    [MemoryDiagnoser]
    public class RenderNested
    {
        private HandlebarsTemplate<TextWriter, object, object> _template;
        private object _data;

        private const int SectionCount = 5;

        private const string Source =
            "{{title}}\n" +
            "Period: {{period.start}} to {{period.end}}\n" +
            "Generated: {{generatedAt}}\n" +
            "{{#each sections}}\n" +
            "[{{name}}]\n" +
            "{{#each metrics}}" +
            "  {{label}}: {{value}}{{#if unit}} {{unit}}{{/if}}{{#unless withinTarget}} [ALERT]{{/unless}}\n" +
            "{{/each}}" +
            "{{/each}}\n" +
            "Totals: {{summary.totalMetrics}} metrics, {{summary.alertCount}} alerts";

        [Params(5, 20)]
        public int RowsPerSection { get; set; }

        [Params("object", "dictionary")]
        public string DataType { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var handlebars = Handlebars.Create();
            _template = handlebars.Compile(new StringReader(Source));

            _data = DataType switch
            {
                "object" => BuildObjectData(),
                _        => BuildDictionaryData()
            };
        }

        [Benchmark]
        public void Render() => _template(TextWriter.Null, _data);

        private static readonly string[] SectionNames = ["Traffic", "Revenue", "Errors", "Performance", "Conversion"];
        private static readonly string[] MetricLabels = ["Count", "Rate", "p50", "p99", "Total", "Delta", "Avg", "Max", "Min", "StdDev",
                                                          "Score", "Index", "Ratio", "Share", "Volume", "Trend", "Baseline", "Peak", "Trough", "Median"];
        private static readonly string[] Units = ["req/s", "%", "ms", "$", null, null]; // nulls make ~half unitless

        private object BuildObjectData()
        {
            int alertCount = 0;
            var sections = new List<object>(SectionCount);
            for (int s = 0; s < SectionCount; s++)
            {
                var metrics = new List<object>(RowsPerSection);
                for (int r = 0; r < RowsPerSection; r++)
                {
                    bool withinTarget = (s * RowsPerSection + r) % 4 != 0;
                    if (!withinTarget) alertCount++;
                    string unit = Units[(s * RowsPerSection + r) % Units.Length];
                    metrics.Add(new
                    {
                        label        = MetricLabels[(s * RowsPerSection + r) % MetricLabels.Length],
                        value        = $"{42.0 + s * 10 + r:F1}",
                        unit         = unit,
                        withinTarget = withinTarget
                    });
                }
                sections.Add(new { name = SectionNames[s], metrics });
            }

            return new
            {
                title       = "Operations Dashboard",
                period      = new { start = "June 1, 2026", end = "June 18, 2026" },
                generatedAt = "June 19, 2026 00:00 UTC",
                sections,
                summary     = new { totalMetrics = SectionCount * RowsPerSection, alertCount }
            };
        }

        private Dictionary<string, object> BuildDictionaryData()
        {
            int alertCount = 0;
            var sections = new List<Dictionary<string, object>>(SectionCount);
            for (int s = 0; s < SectionCount; s++)
            {
                var metrics = new List<Dictionary<string, object>>(RowsPerSection);
                for (int r = 0; r < RowsPerSection; r++)
                {
                    bool withinTarget = (s * RowsPerSection + r) % 4 != 0;
                    if (!withinTarget) alertCount++;
                    string unit = Units[(s * RowsPerSection + r) % Units.Length];
                    metrics.Add(new Dictionary<string, object>
                    {
                        ["label"]        = MetricLabels[(s * RowsPerSection + r) % MetricLabels.Length],
                        ["value"]        = $"{42.0 + s * 10 + r:F1}",
                        ["unit"]         = unit,
                        ["withinTarget"] = withinTarget
                    });
                }
                sections.Add(new Dictionary<string, object>
                {
                    ["name"]    = SectionNames[s],
                    ["metrics"] = metrics
                });
            }

            return new Dictionary<string, object>
            {
                ["title"]       = "Operations Dashboard",
                ["period"]      = new Dictionary<string, object> { ["start"] = "June 1, 2026", ["end"] = "June 18, 2026" },
                ["generatedAt"] = "June 19, 2026 00:00 UTC",
                ["sections"]    = sections,
                ["summary"]     = new Dictionary<string, object>
                {
                    ["totalMetrics"] = SectionCount * RowsPerSection,
                    ["alertCount"]   = alertCount
                }
            };
        }
    }
}
