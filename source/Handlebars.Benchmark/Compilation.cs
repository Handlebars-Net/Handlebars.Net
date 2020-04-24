using System;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using HandlebarsDotNet;
using HandlebarsDotNet.Extension.CompileFast;

namespace Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class Compilation
    {
        private IHandlebars _handlebars;

        [Params("current", "current-fast", "1.10.1")]
        public string Version;
        
        private Func<string, Func<object, string>> _compileMethod;
        private object _handlebarsRef;

        [GlobalSetup]
        public void Setup()
        {
            if (!Version.StartsWith("current"))
            {
                var assembly = Assembly.LoadFile($"{Environment.CurrentDirectory}\\PreviousVersion\\{Version}.dll");
                var type = assembly.GetTypes().FirstOrDefault(o => o.Name == nameof(Handlebars));
                var methodInfo = type.GetMethod("Create");
                _handlebarsRef = methodInfo.Invoke(null, new object[]{ null });
                var objType = _handlebarsRef.GetType();

                var compileMethod = objType.GetMethod("Compile", new[]{typeof(string)});
                _compileMethod = (Func<string, Func<object, string>>) compileMethod.CreateDelegate(typeof(Func<string, Func<object, string>>), _handlebarsRef);
            }
            else
            {
                _handlebars = Handlebars.Create();
                if (Version.Contains("fast"))
                {
                    _handlebars.Configuration.UseCompileFast();
                }

                _compileMethod = _handlebars.Compile;
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
            
            return Compile(template);
        }

        private Func<object, string> Compile(string template)
        {
            return _compileMethod(template);
        }
    }
}