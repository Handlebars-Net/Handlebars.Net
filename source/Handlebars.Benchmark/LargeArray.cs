using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;

namespace HandlebarsNet.Benchmark
{
    public class LargeArray
    {
        private object _data;
        private HandlebarsTemplate<object, object> _default;

        [Params(20000, 40000, 80000)]
        public int N { get; set; }
        
        [GlobalSetup]
        public void Setup()
        {
            const string template = @"{{#each this}}{{this}}{{/each}}";
            _default = Handlebars.Compile(template);
            _data = Enumerable.Range(0, N).ToList();
        }
        
        [Benchmark]
        public void Default() => _default(_data);
    }
}