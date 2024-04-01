﻿using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Helpers;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class SubExpressionVisitor : HandlebarsExpressionVisitor
    {
        private CompilationContext CompilationContext { get; }

        public SubExpressionVisitor(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitSubExpression(SubExpressionExpression subex)
        {
            switch (subex.Expression)
            {
                case MethodCallExpression callExpression:
                    return HandleMethodCallExpression(callExpression);
                
                default:
                    var expression = FunctionBuilder.Reduce(subex.Expression, CompilationContext, out _);
                    if (expression is MethodCallExpression lateBoundCall)
                        return HandleMethodCallExpression(lateBoundCall);
                    
                    throw new HandlebarsCompilerException("Sub-expression does not contain a converted MethodCall expression");
            }
        }

        private static Expression HandleMethodCallExpression(MethodCallExpression helperCall)
        {
            var options = Arg<HelperOptions>(helperCall.Arguments[1]);
            var context = Arg<Context>(helperCall.Arguments[2]);
            var arguments = Arg<Arguments>(helperCall.Arguments[3]);
            var helper = Arg<IHelperDescriptor<HelperOptions>>(helperCall.Object);

            return helper.Call(o => o.Invoke(options, context, arguments));
        }
    }
}

