using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;
using HandlebarsDotNet.Extension.CompileFast;
using Newtonsoft.Json.Linq;

namespace HandlebarsNet.Benchmark
{
    public class Execution
    {
        private object _data;
        private Action<TextWriter, object> _template;

        [Params(2, 5, 10)]
        public int N { get; }

        [Params("current", "current-cache", "current-fast", "current-fast-cache")]
        public string Version { get; }

        [Params("object", "dictionary", "json")]
        public string DataType { get; }

        [GlobalSetup]
        public void Setup()
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

            switch (DataType)
            {
                case "object":
                    _data = new { level1 = ObjectLevel1Generator()};
                    break;
                
                case "dictionary":
                    _data = new Dictionary<string, object>{["level1"] = DictionaryLevel1Generator()};
                    break;
                
                case "json":
                    _data = new JObject {["level1"] = JsonLevel1Generator()};
                    break;
            }

            var handlebars = Handlebars.Create();
            handlebars.Configuration.CompileTimeConfiguration.UseAggressiveCaching = Version.Contains("cache");

            if (Version.Contains("fast"))
            {
                handlebars.Configuration.UseCompileFast();
            }

            using (var reader = new StringReader(template))
            {
                _template = handlebars.Compile(reader);
            }

            List<object> ObjectLevel1Generator()
            {
                var level = new List<object>();
                for (int i = 0; i < N; i++)
                {
                    level.Add(new
                    {
                        id = $"{i}",
                        level2 = ObjectLevel2Generator(i)
                    });
                }

                return level;
            }
            
            List<object> ObjectLevel2Generator(int id1)
            {
                var level = new List<object>();
                for (int i = 0; i < N; i++)
                {
                    level.Add(new
                    {
                        id = $"{id1}-{i}",
                        level3 = ObjectLevel3Generator(id1, i)
                    });
                }

                return level;
            }
            
            List<object> ObjectLevel3Generator(int id1, int id2)
            {
                var level = new List<object>();
                for (int i = 0; i < N; i++)
                {
                    level.Add(new
                    {
                        id = $"{id1}-{id2}-{i}"
                    });
                }

                return level;
            }

            List<Dictionary<string, object>> DictionaryLevel1Generator()
            {
                var level = new List<Dictionary<string, object>>();
                for (int i = 0; i < N; i++)
                {
                    level.Add(new Dictionary<string, object>()
                    {
                        ["id"] = $"{i}",
                        ["level2"] = DictionaryLevel2Generator(i)
                    });
                }

                return level;
            }
            
            List<Dictionary<string, object>> DictionaryLevel2Generator(int id1)
            {
                var level = new List<Dictionary<string, object>>();
                for (int i = 0; i < N; i++)
                {
                    level.Add(new Dictionary<string, object>()
                    {
                        ["id"] = $"{id1}-{i}",
                        ["level3"] = DictionaryLevel3Generator(id1, i)
                    });
                }

                return level;
            }
            
            List<Dictionary<string, object>> DictionaryLevel3Generator(int id1, int id2)
            {
                var level = new List<Dictionary<string, object>>();
                for (int i = 0; i < N; i++)
                {
                    level.Add(new Dictionary<string, object>()
                    {
                        ["id"] = $"{id1}-{id2}-{i}"
                    });
                }

                return level;
            }

            JArray JsonLevel1Generator()
            {
                var level = new JArray();
                for (int i = 0; i < N; i++)
                {
                    level.Add(new JObject
                    {
                        ["id"] = $"{i}",
                        ["level2"] = JsonLevel2Generator(i)
                    });
                }

                return level;
            }
            
            JArray JsonLevel2Generator(int id1)
            {
                var level = new JArray();
                for (int i = 0; i < N; i++)
                {
                    level.Add(new JObject
                    {
                        ["id"] = $"{id1}-{i}",
                        ["level3"] = JsonLevel3Generator(id1, i)
                    });
                }

                return level;
            }
            
            JArray JsonLevel3Generator(int id1, int id2)
            {
                var level = new JArray();
                for (int i = 0; i < N; i++)
                {
                    level.Add(new JObject()
                    {
                        ["id"] = $"{id1}-{id2}-{i}"
                    });
                }

                return level;
            }
        }
        
        [Benchmark]
        public void Render()
        {
            _template(TextWriter.Null, _data);
        }
    }
}