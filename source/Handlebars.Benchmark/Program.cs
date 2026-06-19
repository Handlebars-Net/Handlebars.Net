using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace HandlebarsNet.Benchmark
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var job = Job.MediumRun.WithLaunchCount(1);

            var manualConfig = DefaultConfig.Instance
                .AddJob(job);

            manualConfig.AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByMethod);

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, manualConfig);
        }
    }
}
