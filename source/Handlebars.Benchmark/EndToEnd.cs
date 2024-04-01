using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsNet.Benchmark
{
    public class EndToEnd
    {
        private object _data;
        private HandlebarsTemplate<TextWriter, object, object> _default;

        [Params(5)]
        public int N { get; set; }
        
        [Params("object", "dictionary")]
        public string DataType { get; set; }
        
        [GlobalSetup]
        public void Setup()
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

            switch (DataType)
            {
                case "object":
                    _data = new { level1 = ObjectLevel1Generator()};
                    break;
                
                case "dictionary":
                    _data = new Dictionary<string, object>{["level1"] = DictionaryLevel1Generator()};
                    break;
            }

            var handlebars = Handlebars.Create();
            using(handlebars.Configure())
            {
                handlebars.RegisterHelper(new PowHelper("pow1"));
                handlebars.RegisterHelper(new PowHelper("pow2"));
                handlebars.RegisterHelper(new BlockPowHelper("pow5"));
            }

            using (var reader = new StringReader(template))
            {
                _default = handlebars.Compile(reader);
            }

            using(handlebars.Configure())
            {
                handlebars.RegisterHelper(new PowHelper("pow3"));
                handlebars.RegisterHelper(new PowHelper("pow4"));
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
        }
        
        private class PowHelper : IHelperDescriptor<HelperOptions>
        {
            public PowHelper(PathInfo name) => Name = name;

            public PathInfo Name { get; }

            public object Invoke(in HelperOptions options, in Context context, in Arguments arguments)
            {
                return ((int)arguments[0] * (int)arguments[0]).ToString();
            }

            public void Invoke(in EncodedTextWriter output, in HelperOptions options, in Context context, in Arguments arguments)
            {
                output.WriteSafeString(((int)arguments[0] * (int)arguments[0]).ToString());
            }
        }
        
        private class BlockPowHelper : IHelperDescriptor<BlockHelperOptions>
        {
            public BlockPowHelper(PathInfo name) => Name = name;

            public PathInfo Name { get; }

            public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
            {
                return ((int)arguments[0] * (int)arguments[0]).ToString();
            }

            public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
            {
                output.WriteSafeString(((int)arguments[0] * (int)arguments[0]).ToString());
            }
        }
        
        [Benchmark]
        public void Default() => _default(TextWriter.Null, _data);
    }
}