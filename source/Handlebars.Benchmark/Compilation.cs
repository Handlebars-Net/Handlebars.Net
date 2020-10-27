using System;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;

namespace HandlebarsNet.Benchmark
{
    public class Compilation
    {
        private IHandlebars _handlebars;
        
        [GlobalSetup]
        public void Setup()
        {
            _handlebars = Handlebars.Create();
            
            _handlebars.RegisterHelper("pow1",  (output, context, arguments) => output.WriteSafeString(((int)arguments[0] * (int) arguments[0]).ToString()));
            _handlebars.RegisterHelper("pow2", (output, context, arguments) => output.WriteSafeString(((int)arguments[0] * (int) arguments[0]).ToString()));
            _handlebars.RegisterHelper("pow5", (output, options, context, arguments) => output.WriteSafeString(((int)arguments[0] * (int) arguments[0]).ToString()));
        }

        [Benchmark]
        public HandlebarsTemplate<object, object> Template()
        {
            const string template = @"
                childCount={{level1.Count}}
                {{#each level1}}
                    id={{id}}
                    childCount={{level2.Count}}
                    index=[{{@../../index}}:{{@../index}}:{{@index}}]
                    pow1=[{{pow1 @index}}]
                    pow2=[{{pow2 @index}}]
                    pow3=[{{pow3 @index}}]
                    pow4=[{{pow4 @index}}]
                    pow5=[{{#pow5 @index}}empty{{/pow5}}]
                    {{#each level2}}
                        id={{id}}
                        childCount={{level3.Count}}
                        index=[{{@../../index}}:{{@../index}}:{{@index}}]
                        pow1=[{{pow1 @index}}]
                        pow2=[{{pow2 @index}}]
                        pow3=[{{pow3 @index}}]
                        pow4=[{{pow4 @index}}]
                        pow5=[{{#pow5 @index}}empty{{/pow5}}]
                        {{#each level3}}
                            id={{id}}
                            index=[{{@../../index}}:{{@../index}}:{{@index}}]
                            pow1=[{{pow1 @index}}]
                            pow2=[{{pow2 @index}}]
                            pow3=[{{pow3 @index}}]
                            pow4=[{{pow4 @index}}]
                            pow5=[{{#pow5 @index}}empty{{/pow5}}]
                        {{/each}}
                    {{/each}}    
                {{/each}}";
            
            return _handlebars.Compile(template);
        }
    }
}