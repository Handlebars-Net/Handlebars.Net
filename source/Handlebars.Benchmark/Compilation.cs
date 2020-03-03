using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using HandlebarsDotNet;

namespace Benchmark
{
    //[SimpleJob(RuntimeMoniker.Net461)]
    [SimpleJob(RuntimeMoniker.NetCoreApp21, baseline: true)]
    public class Compilation
    {
        private IHandlebars _handlebars;
        
        [GlobalSetup]
        public void Setup()
        {
            _handlebars = Handlebars.Create();
            _handlebars.RegisterHelper("customHelper", (writer, context, parameters) =>
            {
            });
        }

        [Benchmark]
        public Func<object, string> Complex()
        {
            const string template = "{{#each County}}" +
                                    "{{#if @first}}" +
                                    "{{this}}" +
                                    "{{else}}" +
                                    "{{#if @last}}" +
                                    " and {{this}}" +
                                    "{{else}}" +
                                    ", {{this}}" +
                                    "{{/if}}" +
                                    "{{/if}}" +
                                    "{{/each}}";

            return _handlebars.Compile(template);
        }

        [Benchmark]
        public Func<object, string> WithParentIndex()
        {
            const string template = @"
                {{#each level1}}
                    id={{id}}
                    index=[{{@../../index}}:{{@../index}}:{{@index}}]
                    first=[{{@../../first}}:{{@../first}}:{{@first}}]
                    last=[{{@../../last}}:{{@../last}}:{{@last}}]
                    {{#each level2}}
                        id={{id}}
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

        [Benchmark]
        public Func<object, string> Each()
        {
            const string template = "{{#each enumerateMe}}{{this}} {{/each}}";
            return _handlebars.Compile(template);
        }
        
        // [Benchmark]
        // public Func<object, string> EachBlockParams()
        // {
        //     const string template = "{{#each enumerateMe as |item val|}}{{item}} {{val}} {{/each}}";
        //     return _handlebars.Compile(template);
        // }
        
        [Benchmark]
        public Func<object, string> Helper()
        {
            const string source = @"{{customHelper 'value'}}";

            return _handlebars.Compile(source);
        }
        
        [Benchmark]
        public Func<object, string> HelperPostRegister()
        {
            const string source = @"{{not_registered_helper 'value'}}";

            return _handlebars.Compile(source);
        }
    }
}