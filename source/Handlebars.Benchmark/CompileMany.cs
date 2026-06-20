using System.IO;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;

namespace HandlebarsNet.Benchmark
{
    // Measures the cost of compiling N templates against a single long-lived environment —
    // the pattern used by production apps and by the static Handlebars.Compile() singleton.
    //
    // Each Compile() call subscribes a new FormatterProvider and ObjectDescriptorFactory to
    // the environment's ObservableList/ObservableIndex observers via WeakCollection. If dead
    // WeakReference slots are not reclaimed, WeakCollection.Add() must scan an ever-growing
    // list before finding a slot, making compilation O(N²) over the environment lifetime.
    //
    // Per-compile time and allocation should be O(1): the N=100 row should be ~10× the
    // N=10 row. A larger ratio (≥50×) indicates observer-slot accumulation has regressed
    // into O(N²) scan behaviour. N=1000 is intentionally omitted: with [IterationSetup]
    // forcing InvocationCount=1, MediumRun × 1000 compilations exceeds CI time budgets.
    [MemoryDiagnoser]
    public class CompileMany
    {
        private IHandlebars _env;

        // A realistic multi-expression template; keeps compilation non-trivial.
        private const string Template =
            "Dear {{user.firstName}} {{user.lastName}},\n" +
            "Your order #{{order.id}} placed on {{order.date}} is now {{order.status}}.\n" +
            "{{#if order.isPaid}}Payment confirmed.{{else}}Payment pending.{{/if}}\n" +
            "{{#each order.items}}  - {{name}} x{{qty}}\n{{/each}}" +
            "Ship to: {{address.city}}, {{address.country}}";

        [Params(10, 100)]
        public int N { get; set; }

        // IterationSetup (not GlobalSetup) so each BDN measurement starts from a clean
        // environment with zero prior subscriptions. The N compilations within one call
        // accumulate subscriptions from 0 → N, revealing any O(N²) scan cost.
        [IterationSetup]
        public void Setup() => _env = Handlebars.Create();

        [Benchmark]
        public void Compile()
        {
            for (var i = 0; i < N; i++)
                _ = _env.Compile(new StringReader(Template));
        }
    }
}
