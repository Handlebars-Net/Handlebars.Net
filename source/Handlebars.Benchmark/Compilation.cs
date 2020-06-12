using System;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using HandlebarsDotNet;
using HandlebarsDotNet.Extension.CompileFast;

namespace HandlebarsNet.Benchmark
{
    public class Compilation
    {
        private IHandlebars _handlebars;

        [Params("current", "current-fast")]
        public string Version { get; }
        
        [GlobalSetup]
        public void Setup()
        {
            _handlebars = Handlebars.Create();
            if (Version.Contains("fast"))
            {
                _handlebars.Configuration.UseCompileFast();
            }
        }

        [Benchmark]
        public Func<object, string> Template()
        {
            const string template = @"
                childCount={{level1.Count}}
                childCount2={{level1.Count}}
                {{#each level1}}
                    id={{id}}
                    childCount={{level2.Count}}
                    childCount2={{level2.Count}}
                    index=[{{@../../index}}:{{@../index}}:{{@index}}]
                    first=[{{@../../first}}:{{@../first}}:{{@first}}]
                    last=[{{@../../last}}:{{@../last}}:{{@last}}]
                    {{#each level2}}
                        id={{id}}
                        childCount={{level3.Count}}
                        childCount2={{level3.Count}}
                        index=[{{@../../index}}:{{@../index}}:{{@index}}]
                        first=[{{@../../first}}:{{@../first}}:{{@first}}]
                        last=[{{@../../last}}:{{@../last}}:{{@last}}]
                        {{#each level3}}
                            id={{id}}
                            index=[{{@../../index}}:{{@../index}}:{{@index}}]
                            first=[{{@../../first}}:{{@../first}}:{{@first}}]
                            last=[{{@../../last}}:{{@../last}}:{{@last}}]
                        {{/each}}
                    {{/each}}    
                {{/each}}";
            
            return _handlebars.Compile(template);
        }
    }
}