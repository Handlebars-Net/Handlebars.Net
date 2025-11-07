using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;

namespace HandlebarsNet.Benchmark
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var job = Job.MediumRun
                .WithToolchain(CsProjCoreToolchain.NetCoreApp80)
                .WithLaunchCount(1);

            var manualConfig = DefaultConfig.Instance
                .AddJob(job);

            manualConfig.AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByMethod);
            
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, manualConfig);
        }
    }
}