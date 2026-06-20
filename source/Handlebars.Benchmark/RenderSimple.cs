using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;

namespace HandlebarsNet.Benchmark
{
    // Render time for a notification-style template: nested property access,
    // two if/else blocks, no loops. Baseline scenario for regression detection.
    [MemoryDiagnoser]
    public class RenderSimple
    {
        private HandlebarsTemplate<TextWriter, object, object> _template;
        private object _data;

        private const string Source =
            "Dear {{user.firstName}} {{user.lastName}},\n\n" +
            "Your order #{{order.number}} placed on {{order.date}} is now {{order.status}}.\n" +
            "{{#if order.isPaid}}Payment confirmed.{{else}}Payment pending.{{/if}}\n" +
            "{{#if order.hasTracking}}Tracking: {{order.trackingNumber}} via {{order.carrier}}\n{{/if}}\n" +
            "Ship to: {{address.line1}}, {{address.city}}, {{address.state}} {{address.zip}}, {{address.country}}\n\n" +
            "— The {{store.name}} Team";

        [Params("object", "dictionary", "expando")]
        public string DataType { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var handlebars = Handlebars.Create();
            _template = handlebars.Compile(new StringReader(Source));

            _data = DataType switch
            {
                "object"     => BuildObject(),
                "dictionary" => BuildDictionary(),
                _            => BuildExpando()
            };
        }

        [Benchmark]
        public void Render() => _template(TextWriter.Null, _data);

        private static object BuildObject() => new
        {
            user = new { firstName = "Alice", lastName = "Johnson" },
            order = new
            {
                number = "ORD-00421",
                date = "June 18, 2026",
                status = "Shipped",
                isPaid = true,
                hasTracking = true,
                trackingNumber = "1Z999AA10123456784",
                carrier = "UPS"
            },
            address = new
            {
                line1 = "123 Main St",
                city = "Springfield",
                state = "IL",
                zip = "62701",
                country = "USA"
            },
            store = new { name = "Acme Shop" }
        };

        private static Dictionary<string, object> BuildDictionary() => new()
        {
            ["user"] = new Dictionary<string, object>
            {
                ["firstName"] = "Alice",
                ["lastName"] = "Johnson"
            },
            ["order"] = new Dictionary<string, object>
            {
                ["number"] = "ORD-00421",
                ["date"] = "June 18, 2026",
                ["status"] = "Shipped",
                ["isPaid"] = true,
                ["hasTracking"] = true,
                ["trackingNumber"] = "1Z999AA10123456784",
                ["carrier"] = "UPS"
            },
            ["address"] = new Dictionary<string, object>
            {
                ["line1"] = "123 Main St",
                ["city"] = "Springfield",
                ["state"] = "IL",
                ["zip"] = "62701",
                ["country"] = "USA"
            },
            ["store"] = new Dictionary<string, object>
            {
                ["name"] = "Acme Shop"
            }
        };

        private static ExpandoObject BuildExpando()
        {
            dynamic user = new ExpandoObject();
            user.firstName = "Alice"; user.lastName = "Johnson";

            dynamic order = new ExpandoObject();
            order.number = "ORD-00421"; order.date = "June 18, 2026"; order.status = "Shipped";
            order.isPaid = true; order.hasTracking = true;
            order.trackingNumber = "1Z999AA10123456784"; order.carrier = "UPS";

            dynamic address = new ExpandoObject();
            address.line1 = "123 Main St"; address.city = "Springfield"; address.state = "IL";
            address.zip = "62701"; address.country = "USA";

            dynamic store = new ExpandoObject();
            store.name = "Acme Shop";

            dynamic root = new ExpandoObject();
            root.user = user; root.order = order; root.address = address; root.store = store;
            return (ExpandoObject)root;
        }
    }
}
