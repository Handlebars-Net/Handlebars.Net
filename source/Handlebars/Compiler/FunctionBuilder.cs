using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal static class FunctionBuilder
    {
        private static readonly Expression<Action<TextWriter, object>> EmptyLambda =
            Expression.Lambda<Action<TextWriter, object>>(
                Expression.Empty(),
                Expression.Parameter(typeof(TextWriter)),
                Expression.Parameter(typeof(object)));
        
        private static readonly Action<BindingContext, TextWriter, object> EmptyLambdaWithContext = (context, writer, arg3) => {};

        public static Expression Reduce(Expression expression, CompilationContext context)
        {
            expression = new CommentVisitor().Visit(expression);
            expression = new UnencodedStatementVisitor(context).Visit(expression);
            expression = new PartialBinder(context).Visit(expression);
            expression = new StaticReplacer(context).Visit(expression);
            expression = new IteratorBinder(context).Visit(expression);
            expression = new BlockHelperFunctionBinder(context).Visit(expression);
            expression = new HelperFunctionBinder(context).Visit(expression);
            expression = new BoolishConverter(context).Visit(expression);
            expression = new PathBinder(context).Visit(expression);
            expression = new SubExpressionVisitor(context).Visit(expression);
            expression = new HashParameterBinder().Visit(expression);

            return expression;
        }

        public static Action<BindingContext, TextWriter, object> CompileCore(IEnumerable<Expression> expressions, ICompiledHandlebarsConfiguration configuration, string templatePath = null)
        {
            try
            {
                if (!expressions.Any())
                {
                    return EmptyLambdaWithContext;
                }
                if (expressions.IsOneOf<Expression, DefaultExpression>())
                {
                    return EmptyLambdaWithContext;
                }
                
                var context = new CompilationContext(configuration);
                var expression = (Expression) Expression.Block(expressions);
                expression = Reduce(expression, context);

                var lambda = ContextBinder.Bind(context, expression, templatePath);
                return configuration.ExpressionCompiler.Compile(lambda);
            }
            catch (Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }
        
        public static Expression<Action<TextWriter, object>> CompileCore(IEnumerable<Expression> expressions, Expression parentContext, ICompiledHandlebarsConfiguration configuration, string templatePath = null)
        {
            try
            {
                if (!expressions.Any())
                {
                    return EmptyLambda;
                }
                if (expressions.IsOneOf<Expression, DefaultExpression>())
                {
                    return EmptyLambda;
                }
                
                var context = new CompilationContext(configuration);
                var expression = (Expression) Expression.Block(expressions);
                expression = Reduce(expression, context);

                return ContextBinder.Bind(context, expression, parentContext, templatePath);
            }
            catch (Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }

        public static Action<TextWriter, object> Compile(IEnumerable<Expression> expressions, ICompiledHandlebarsConfiguration configuration, string templatePath = null)
        {
            try
            {
                var expression = CompileCore(expressions, null, configuration,  templatePath);

                return configuration.ExpressionCompiler.Compile(expression);
            }
            catch (Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }
    }
}

