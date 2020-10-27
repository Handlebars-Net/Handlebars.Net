using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Expressions.Shortcuts;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal static class FunctionBuilder
    {
        private static readonly TemplateDelegate EmptyLambda = 
            (in EncodedTextWriter writer, BindingContext context) => { };

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

        public static ExpressionContainer<TemplateDelegate> CreateExpression(IEnumerable<Expression> expressions, ICompiledHandlebarsConfiguration configuration)
        {
            try
            {
                var enumerable = expressions as Expression[] ?? expressions.ToArray();
                if (!enumerable.Any())
                {
                    return Arg(EmptyLambda);
                }
                if (enumerable.IsOneOf<Expression, DefaultExpression>())
                {
                    return Arg(EmptyLambda);
                }
                
                var context = new CompilationContext(configuration);
                var expression = (Expression) Expression.Block(enumerable);
                expression = Reduce(expression, context);

                return Arg(ContextBinder.Bind(context, expression));
            }
            catch (Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }

        public static TemplateDelegate Compile(IEnumerable<Expression> expressions, ICompiledHandlebarsConfiguration configuration)
        {
            try
            {
                var expression = CreateExpression(expressions, configuration);
                if (expression.Expression is ConstantExpression constantExpression)
                {
                    return (TemplateDelegate) constantExpression.Value;
                }

                var lambda = (Expression<TemplateDelegate>) expression.Expression;
                return configuration.ExpressionCompiler.Compile(lambda);
            }
            catch (Exception ex)
            {
                throw new HandlebarsCompilerException("An unhandled exception occurred while trying to compile the template", ex);
            }
        }
    }
}

