using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Handlebars.Compiler.Lexer;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class FunctionBuilder
    {
        private readonly HandlebarsConfiguration _configuration;

        public FunctionBuilder(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Action<TextWriter, object> Compile(IEnumerable<object> tokens)
        {
            try
            {
                var expressions = ConvertTokensToExpressions(tokens);
                var expression = CreateExpressionBlock(expressions);
                expression = StaticReplacer.Replace(expression);
                expression = BlockHelperFunctionBinder.Bind(expression, _configuration);
                expression = HelperFunctionBinder.Bind(expression, _configuration);
                expression = PathBinder.Bind(expression);
                expression = ContextBinder.Bind(expression);
                expression = FunctionWrapper.Wrap(expression);
                return ((Expression<Action<TextWriter, object>>)expression).Compile();
            }
            catch(Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }

        private IEnumerable<Expression> ConvertTokensToExpressions(IEnumerable<object> tokens)
        {
            tokens = StaticConverter.Convert(tokens);
            tokens = LiteralConverter.Convert(tokens);
            tokens = HelperConverter.Convert(tokens, _configuration);
            tokens = PathConverter.Convert(tokens);
            tokens = HelperArgumentAccumulator.Accumulate(tokens);
            tokens = ExpressionScopeConverter.Convert(tokens);
            tokens = BlockHelperAccumulator.Accumulate(tokens, _configuration);
            return tokens.Cast<Expression>();
        }

        private Expression CreateExpressionBlock(IEnumerable<Expression> expressions)
        {
            return Expression.Block(expressions);
        }
    }
}

