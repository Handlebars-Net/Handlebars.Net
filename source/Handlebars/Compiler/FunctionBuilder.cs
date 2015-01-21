﻿using System;
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
        private static readonly Expression _emptyLambda =
            Expression.Lambda<Action<TextWriter, object>>(
                Expression.Empty(),
                Expression.Parameter(typeof(TextWriter)),
                Expression.Parameter(typeof(object)));

        public FunctionBuilder(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Expression Compile(IEnumerable<Expression> expressions, Expression parentContext)
        {
            try
            {
                if (expressions.Any() == false)
                {
                    return _emptyLambda;
                }
                if (expressions.IsOneOf<Expression, DefaultExpression>() == true)
                {
                    return _emptyLambda;
                }
                var compilationContext = new CompilationContext(_configuration);
                var expression = CreateExpressionBlock(expressions);
                expression = StaticReplacer.Replace(expression, compilationContext);
                expression = PartialBinder.Bind(expression, compilationContext);
                expression = IteratorBinder.Bind(expression, compilationContext);
                expression = BlockHelperFunctionBinder.Bind(expression, compilationContext);
                expression = DeferredSectionVisitor.Bind(expression, compilationContext);
                expression = HelperFunctionBinder.Bind(expression, compilationContext);
                expression = BoolishConverter.Convert(expression, compilationContext);
                expression = PathBinder.Bind(expression, compilationContext);
                expression = ContextBinder.Bind(expression, compilationContext, parentContext);


                return expression;
            }
            catch (Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }

        public Action<TextWriter, object> Compile(IEnumerable<Expression> expressions)
        {
            try
            {
                var expression = Compile(expressions, null);
                return ((Expression<Action<TextWriter, object>>)expression).Compile();
            }
            catch (Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }


        private Expression CreateExpressionBlock(IEnumerable<Expression> expressions)
        {
            return Expression.Block(expressions);
        }
    }
}

