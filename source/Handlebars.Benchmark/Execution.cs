using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Bogus;
using HandlebarsDotNet;
using Newtonsoft.Json.Linq;

namespace Benchmark
{
    [SimpleJob(RuntimeMoniker.Net461)]
    [SimpleJob(RuntimeMoniker.NetCoreApp21, baseline: true)]
    public class Execution
    {
        private IHandlebars _handlebars;
        
        [Params(10000)]
        public int N;
        
        private Faker _faker;
        private Func<object, string>[] _templates;
        private Dictionary<string, object> _arrayData;
        private Dictionary<string, object> _dictionaryData;
        private Dictionary<string, object> _objectData;
        private Dictionary<string, object> _jsonData;
        private Type _type;
        private string[] _propertyNames;

        [GlobalSetup]
        public void Setup()
        {
            _handlebars = Handlebars.Create();
            _faker = new Faker();

            const string eachTemplate = "{{#each County}}{{this}} {{/each}}";
            const string blockParamsEach = "{{#each County as |item val|}}{{item}} {{val}} {{/each}}";
            const string complexTemplate = "{{#each County}}" +
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
            const string helperTemplate = "{{customHelper 'value'}}";
            const string notRegisteredHelperTemplate = "{{not_registered_helper 'value'}}";
            
            _handlebars.RegisterHelper("customHelper", (writer, context, parameters) =>
            {
                writer.WriteSafeString(parameters[0]);
            });
            
            string RandomProperty() => new string(
                Enumerable.Range(0, 40)
                    .Select(x => x % 2 == 0 ? _faker.Random.Char('a', 'z') : _faker.Random.Char('A', 'Z'))
                    .ToArray()
            );

            var stringType = typeof(string);
            _propertyNames = Enumerable.Range(0, N).Select(o => RandomProperty()).ToArray();
            _type = new ClassBuilder($"BenchmarkCountry{N}")
                .CreateType(
                    _propertyNames,
                    Enumerable.Range(0, N).Select(o => stringType).ToArray()
                );
            
            _templates = new[]
            {
                _handlebars.Compile(eachTemplate),
                _handlebars.Compile(blockParamsEach),
                _handlebars.Compile(complexTemplate),
                _handlebars.Compile(helperTemplate),
                _handlebars.Compile(notRegisteredHelperTemplate),
            };
        }

        [IterationSetup(Targets = new[]{nameof(SimpleEachJsonInput), nameof(EachBlockParamsJsonInput), nameof(ComplexJsonInput)})]
        public void JsonIterationSetup()
        {
            var json = new JObject();
            for (var index = 0; index < _propertyNames.Length; index++)
            {
                json.Add(_propertyNames[index], JToken.FromObject(Guid.NewGuid()));
            }
            
            _jsonData = new Dictionary<string, object>
            {
                ["Country"] = json
            };
        }
        
        [IterationSetup(Targets = new[]{nameof(SimpleEachDictionaryInput), nameof(EachBlockParamsDictionaryInput), nameof(ComplexDictionaryInput)})]
        public void DictionaryIterationSetup()
        {
            _dictionaryData = new Dictionary<string, object>
            {
                ["Country"] = _propertyNames.ToDictionary(o => o, o => Guid.NewGuid().ToString())
            };
        }
        
        [IterationSetup(Targets = new[]{nameof(SimpleEachArrayInput), nameof(EachBlockParamsArrayInput), nameof(ComplexArrayInput)})]
        public void ArrayIterationSetup()
        {
            _arrayData = new Dictionary<string, object>
            {
                ["Country"] = _propertyNames.Select(o => Guid.NewGuid().ToString()).ToArray()
            };
        }
        
        [IterationSetup(Targets = new[]{nameof(SimpleEachObjectInput), nameof(EachBlockParamsObjectInput), nameof(ComplexObjectInput)})]
        public void ObjectIterationSetup()
        {
            var data = Activator.CreateInstance(_type);
            var properties = data.GetType().GetRuntimeProperties().ToArray();
            for (var index = 0; index < properties.Length; index++)
            {
                properties[index].SetValue(data, Guid.NewGuid().ToString());
            }
            
            _objectData = new Dictionary<string, object>
            {
                ["Country"] = data
            };
        }
        
        [IterationSetup(Targets = new[]{nameof(Helper), nameof(HelperPostRegister)})]
        public void HelpersSetup()
        {
        }

        [Benchmark]
        public string SimpleEachArrayInput()
        {
            return _templates[0].Invoke(_arrayData);
        }
        
        [Benchmark]
        public string EachBlockParamsArrayInput()
        {
            return _templates[1].Invoke(_arrayData);
        }
        
        [Benchmark]
        public string ComplexArrayInput()
        {
            return _templates[2].Invoke(_arrayData);
        }
        
        [Benchmark]
        public string SimpleEachObjectInput()
        {
            return _templates[0].Invoke(_objectData);
        }
        
        [Benchmark]
        public string EachBlockParamsObjectInput()
        {
            return _templates[1].Invoke(_objectData);
        }
        
        [Benchmark]
        public string ComplexObjectInput()
        {
            return _templates[2].Invoke(_objectData);
        }
        
        [Benchmark]
        public string SimpleEachJsonInput()
        {
            return _templates[0].Invoke(_jsonData);
        }
        
        [Benchmark]
        public string EachBlockParamsJsonInput()
        {
            return _templates[1].Invoke(_jsonData);
        }
        
        [Benchmark]
        public string ComplexJsonInput()
        {
            return _templates[2].Invoke(_jsonData);
        }
        
        [Benchmark]
        public string SimpleEachDictionaryInput()
        {
            return _templates[0].Invoke(_dictionaryData);
        }
        
        [Benchmark]
        public string EachBlockParamsDictionaryInput()
        {
            return _templates[1].Invoke(_dictionaryData);
        }
        
        [Benchmark]
        public string ComplexDictionaryInput()
        {
            return _templates[2].Invoke(_dictionaryData);
        }
        
        [Benchmark]
        public string Helper()
        {
            return _templates[3].Invoke(new object());
        }
        
        [Benchmark]
        public string HelperPostRegister()
        {
            _handlebars.RegisterHelper("not_registered_helper", (writer, context, parameters) =>
            {
                writer.WriteSafeString(parameters[0]);
            });

            return _templates[4].Invoke(new object());
        }
    }
}