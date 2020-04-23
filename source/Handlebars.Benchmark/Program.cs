using System.Diagnostics;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using HandlebarsDotNet;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var manualConfig = DefaultConfig.Instance.WithArtifactsPath(
                $"./Benchmark-{FileVersionInfo.GetVersionInfo(typeof(Handlebars).Assembly.Location).FileVersion}"
            ).With(BenchmarkLogicalGroupRule.ByMethod);

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, manualConfig);
        }
    }
}