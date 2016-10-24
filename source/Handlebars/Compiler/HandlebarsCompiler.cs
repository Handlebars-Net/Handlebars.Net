﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class HandlebarsCompiler : IHandlebarsCompiler
    {
        private Tokenizer _tokenizer;
        private FunctionBuilder _functionBuilder;
        private ExpressionBuilder _expressionBuilder;
        private HandlebarsConfiguration _configuration;

        public HandlebarsCompiler(HandlebarsConfiguration configuration)
        {
            _configuration=configuration;
            _tokenizer = new Tokenizer(configuration);
            _expressionBuilder = new ExpressionBuilder(configuration);
            _functionBuilder = new FunctionBuilder(configuration);
        }

		public IEnumerable<IToken> Tokenize( TextReader source ) {
		    return _tokenizer.Tokenize( source ).ToList();
		}

		public IEnumerable<Expression> ExpressionBuilder( IEnumerable<IToken> tokens ) {
			return _expressionBuilder.ConvertTokensToExpressions( tokens );
		}

		public Action<TextWriter, object> FunctionBuilder( IEnumerable<Expression> expressions, string templatePath = null ) {
			return _functionBuilder.Compile( expressions, templatePath );
		}

		public Action<TextWriter, object> Compile(TextReader source)
        {
			return FunctionBuilder( ExpressionBuilder( Tokenize( source ) ) );
        }

		public Action<TextWriter, object> CompileView(string templatePath)
        {
            var fs = _configuration.FileSystem;
            if (fs == null) throw new InvalidOperationException("Cannot compile view when configuration.FileSystem is not set");
            var template = fs.GetFileContent(templatePath);
            if (template == null) throw new InvalidOperationException("Cannot find template at '" + templatePath + "'");
            IEnumerable<object> tokens = null;
            using (var sr = new StringReader(template))
            {
                tokens = _tokenizer.Tokenize(sr).ToList();
            }
            var layoutToken = tokens.OfType<LayoutToken>().SingleOrDefault();

            var expressions = _expressionBuilder.ConvertTokensToExpressions(tokens);
            var compiledView = _functionBuilder.Compile(expressions, templatePath);
            if (layoutToken == null) return compiledView;

            var layoutPath = fs.Closest(templatePath, layoutToken.Value + ".hbs");
            if (layoutPath == null) throw new InvalidOperationException("Cannot find layout '" + layoutPath + "' for template '" + templatePath + "'");

            var compiledLayout = CompileView(layoutPath);

            return (tw, vm) =>
            {
                var sb = new StringBuilder();
                using (var innerWriter = new StringWriter(sb))
                {
                    compiledView(innerWriter, vm);
                }
                var inner = sb.ToString();
                compiledLayout(tw, new DynamicViewModel(new {body = inner}));
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
#if netstandard
                return _objects.Select(o => o.GetType())
                   .SelectMany(t => t.GetTypeInfo().GetMembers(BindingFlags))
                   .Select(m => m.Name);
#else
                return _objects.Select(o => o.GetType())
                    .SelectMany(t => t.GetMembers(BindingFlags))
                    .Select(m => m.Name);
#endif
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;
                foreach (var target in _objects)
                {
#if netstandard
                    var member = target.GetType().GetTypeInfo()
                        .GetMember(binder.Name, BindingFlags);
#else
                    var member = target.GetType()
                        .GetMember(binder.Name, BindingFlags);
#endif
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

