using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.IO;
using System.Linq;
using System.Reflection;
using Expressions.Shortcuts;

namespace HandlebarsDotNet.Compiler
{
    // will be significantly improved in next iterations
    internal class SubExpressionVisitor : HandlebarsExpressionVisitor
    {
        private readonly IExpressionCompiler _expressionCompiler;
        private CompilationContext CompilationContext { get; }

        public SubExpressionVisitor(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
            
            _expressionCompiler = CompilationContext.Configuration.CompileTimeConfiguration.ExpressionCompiler;
        }
        
        protected override Expression VisitSubExpression(SubExpressionExpression subex)
        {
            switch (subex.Expression)
            {
                case InvocationExpression invocationExpression:
                    return HandleInvocationExpression(invocationExpression);

                case MethodCallExpression callExpression:
                    return HandleMethodCallExpression(callExpression);
                
                default:
                    var expression = FunctionBuilder.Reduce(subex.Expression, CompilationContext);
                    if (expression is MethodCallExpression lateBoundCall)
                        return HandleMethodCallExpression(lateBoundCall);
                    
                    throw new HandlebarsCompilerException("Sub-expression does not contain a converted MethodCall expression");
            }
        }

        private Expression HandleMethodCallExpression(MethodCallExpression helperCall)
        {
            if (helperCall.Type != typeof(void))
            {
                return helperCall.Update(helperCall.Object, 
                    ReplaceValuesOf<TextWriter>(helperCall.Arguments, ExpressionShortcuts.Null<TextWriter>()).Select(Visit)
                );
            }

            var context = ExpressionShortcuts.Var<BindingContext>();
            var writer = ExpressionShortcuts.Var<TextWriter>();
            helperCall = helperCall.Update(ExpressionUtils.ReplaceParameters(helperCall.Object, context), 
                ExpressionUtils.ReplaceParameters(
                    ReplaceValuesOf<TextWriter>(helperCall.Arguments, writer), new Expression[] { context }
                    ).Select(Visit)
            );

            var formatProvider = ExpressionShortcuts.Arg(CompilationContext.Configuration.FormatProvider);
            var block = ExpressionShortcuts.Block()
                .Parameter(writer, ExpressionShortcuts.New(() => new PolledStringWriter((IFormatProvider) formatProvider)))
                .Line(writer.Using((o, body) =>
                    body.Line(helperCall)
                        .Line(o.Call(x => (object) x.ToString()))
                ));
            
            var continuation = _expressionCompiler.Compile(Expression.Lambda<Func<BindingContext, object>>(block, (ParameterExpression) context));
            return ExpressionShortcuts.Arg<object>(Expression.Invoke(Expression.Constant(continuation), CompilationContext.BindingContext));
        }

        private static IEnumerable<Expression> ReplaceValuesOf<T>(IEnumerable<Expression> instance, Expression newValue)
        {
            var targetType = typeof(T);
            return instance.Select(value => targetType.IsAssignableFrom(value.Type)
                ? newValue
                : value);
        }

        private Expression HandleInvocationExpression(InvocationExpression invocation)
        {
            if (invocation.Type != typeof(void))
            {
                return invocation.Update(invocation.Expression, 
                    ReplaceValuesOf<TextWriter>(invocation.Arguments, ExpressionShortcuts.Null<TextWriter>()).Select(Visit)
                );
            }

            var context = ExpressionShortcuts.Var<BindingContext>();
            var writer = ExpressionShortcuts.Var<TextWriter>();
            invocation = invocation.Update(ExpressionUtils.ReplaceParameters(invocation.Expression, context), 
                ExpressionUtils.ReplaceParameters(
                    ReplaceValuesOf<TextWriter>(invocation.Arguments, writer), new Expression[]{ context }
                    ).Select(Visit)
            );

            var formatProvider = ExpressionShortcuts.Arg(CompilationContext.Configuration.FormatProvider);
            var block = ExpressionShortcuts.Block()
                .Parameter(writer, ExpressionShortcuts.New(() => new PolledStringWriter((IFormatProvider) formatProvider)))
                .Line(writer.Using((o, body) =>
                    body.Line(invocation)
                        .Line(o.Call(x => (object) x.ToString()))
                ));
            
            var continuation = _expressionCompiler.Compile(Expression.Lambda<Func<BindingContext, object>>(block, (ParameterExpression) context));
            return ExpressionShortcuts.Arg<object>(Expression.Invoke(Expression.Constant(continuation), CompilationContext.BindingContext));
        }
    }
}

