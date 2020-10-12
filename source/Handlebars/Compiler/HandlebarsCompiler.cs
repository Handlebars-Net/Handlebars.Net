using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class HandlebarsCompiler
    {
        private readonly HandlebarsConfiguration _configuration;

        public HandlebarsCompiler(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Action<TextWriter, object> Compile(ExtendedStringReader source)
        {
            var configuration = new InternalHandlebarsConfiguration(_configuration);
            var createdFeatures = configuration.Features;
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].OnCompiling(configuration);
            }
            
            var expressionBuilder = new ExpressionBuilder(configuration);
            var tokens = Tokenizer.Tokenize(source).ToList();
            var expressions = expressionBuilder.ConvertTokensToExpressions(tokens);
            var action = FunctionBuilder.Compile(expressions, configuration);
            
            for (var index = 0; index < createdFeatures.Count; index++)
            {
                createdFeatures[index].CompilationCompleted();
            }

            return action;
        }

        internal Action<TextWriter, object> CompileView(ViewReaderFactory readerFactoryFactory, string templatePath, InternalHandlebarsConfiguration configuration)
        {
            IEnumerable<object> tokens;
            using (var sr = readerFactoryFactory(configuration, templatePath))
            {
                using (var reader = new ExtendedStringReader(sr))
                {
                    tokens = Tokenizer.Tokenize(reader).ToList();
                }
            }

            var layoutToken = tokens.OfType<LayoutToken>().SingleOrDefault();

            var expressionBuilder = new ExpressionBuilder(configuration);
            var expressions = expressionBuilder.ConvertTokensToExpressions(tokens);
            var compiledView = FunctionBuilder.Compile(expressions, configuration, templatePath);
            if (layoutToken == null) return compiledView;

            var fs = configuration.FileSystem;
            var layoutPath = fs.Closest(templatePath, layoutToken.Value + ".hbs");
            if (layoutPath == null)
                throw new InvalidOperationException("Cannot find layout '" + layoutPath + "' for template '" +
                                                    templatePath + "'");

            var compiledLayout = CompileView(readerFactoryFactory, layoutPath, configuration);

            return (tw, vm) =>
            {
                string inner;
                using (var innerWriter = new PolledStringWriter(configuration.FormatProvider))
                {
                    compiledView(innerWriter, vm);
                    inner = innerWriter.ToString();
                }

                compiledLayout(tw, new DynamicViewModel(new[] {new {body = inner}, vm}));
            };
        }


        internal class DynamicViewModel : DynamicObject
        {
            private readonly object[] _objects;
            private static readonly BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

            public DynamicViewModel(params object[] objects)
            {
                _objects = objects;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return _objects.Select(o => o.GetType())
                    .SelectMany(t => t.GetMembers(BindingFlags))
                    .Select(m => m.Name);
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;
                foreach (var target in _objects)
                {
                    var member = target.GetType().GetMember(binder.Name, BindingFlags);
                    if (member.Length > 0)
                    {
                        if (member[0] is PropertyInfo)
                        {
                            result = ((PropertyInfo)member[0]).GetValue(target, null);
                            return true;
                        }
                        if (member[0] is FieldInfo)
                        {
                            result = ((FieldInfo)member[0]).GetValue(target);
                            return true;
                        }
                    }
                }
                return false;
            }
        }

    }


}

