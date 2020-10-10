using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using HandlebarsDotNet;

namespace HandlebarsNet.Benchmark
{
    public class Execution
    {
        private readonly List<Action<TextWriter, object>> _templates = new List<Action<TextWriter, object>>();

        [GlobalSetup]
        public void Setup()
        {
            var handlebars = Handlebars.Create();
            
            handlebars.RegisterHelper("helper1", (output, context, arguments) =>
            {
                output.WriteSafeString("42");
            });
            
            handlebars.RegisterHelper("helper2", (output, context, arguments) =>
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    output.WriteSafeString(arguments[i]);
                }
            });

            handlebars.RegisterHelper("helper5", (output, options, context, arguments) =>
            {
                options.Template(output, context);
                output.WriteSafeString("42");
            });
            
            handlebars.RegisterHelper("helper6", (output, options, context, arguments) =>
            {
                options.Template(output, context);
                for (int i = 0; i < arguments.Length; i++)
                {
                    output.WriteSafeString(arguments[i]);
                }
            });
            
            _templates.Add(handlebars.Compile(Read("Call {{helper1}}")));
            _templates.Add(handlebars.Compile(Read("Call {{helper2 '1'}}")));
            _templates.Add(handlebars.Compile(Read("Call {{helper2 '1' '2'}}")));
            
            _templates.Add(handlebars.Compile(Read("Call {{helper3}}")));
            _templates.Add(handlebars.Compile(Read("Call {{helper4 '1'}}")));
            _templates.Add(handlebars.Compile(Read("Call {{helper4 '1' '2'}}")));
            
            _templates.Add(handlebars.Compile(Read("Call {{#helper5}}empty{{/helper5}}")));
            _templates.Add(handlebars.Compile(Read("Call {{#helper6 '1'}}empty{{/helper6}}")));
            _templates.Add(handlebars.Compile(Read("Call {{#helper6 '1' '2'}}empty{{/helper6}}")));
            
            handlebars.RegisterHelper("helper3", (output, context, arguments) =>
            {
                output.WriteSafeString("42");
            });
            
            handlebars.RegisterHelper("helper4", (output, context, arguments) =>
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    output.WriteSafeString(arguments[i]);
                }
            });
        }
        
        [Benchmark]
        public void CallHelperWithoutParameters()
        {
            _templates[0].Invoke(TextWriter.Null, null);
        }
        
        [Benchmark]
        public void CallHelperWithOneParameter()
        {
            _templates[1].Invoke(TextWriter.Null, null);
        }
        
        [Benchmark]
        public void CallHelperWithTwoParameter()
        {
            _templates[2].Invoke(TextWriter.Null, null);
        }
        
        [Benchmark]
        public void LateCallHelperWithoutParameters()
        {
            _templates[3].Invoke(TextWriter.Null, null);
        }
        
        [Benchmark]
        public void LateCallHelperWithOneParameter()
        {
            _templates[4].Invoke(TextWriter.Null, null);
        }
        
        [Benchmark]
        public void LateCallHelperWithTwoParameter()
        {
            _templates[5].Invoke(TextWriter.Null, null);
        }
        
        [Benchmark]
        public void CallBlockHelperWithoutParameters()
        {
            _templates[6].Invoke(TextWriter.Null, null);
        }
        
        [Benchmark]
        public void CallBlockHelperWithOneParameter()
        {
            _templates[7].Invoke(TextWriter.Null, null);
        }
        
        [Benchmark]
        public void CallBlockHelperWithTwoParameter()
        {
            _templates[8].Invoke(TextWriter.Null, null);
        }

        private static TextReader Read(string template) => new StringReader(template);
    }
}