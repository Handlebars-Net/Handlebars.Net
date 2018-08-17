using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperFunctionBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new HelperFunctionBinder(context).Visit(expr);
        }

        private HelperFunctionBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (sex.Body is HelperExpression)
            {
                return Visit(sex.Body);
            }
            else
            {
                return sex;
            }
        }

        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            if (CompilationContext.Configuration.Helpers.ContainsKey(hex.HelperName))
            {
                var helper = CompilationContext.Configuration.Helpers[hex.HelperName];
                var arguments = new Expression[]
                {
                    Expression.Property(
                        CompilationContext.BindingContext,
#if netstandard
                        typeof(BindingContext).GetRuntimeProperty("TextWriter")),
#else
                        typeof(BindingContext).GetProperty("TextWriter")),
#endif
                    Expression.Property(
                        CompilationContext.BindingContext,
#if netstandard
                        typeof(BindingContext).GetRuntimeProperty("Value")),
#else
                        typeof(BindingContext).GetProperty("Value")),
#endif
                    Expression.NewArrayInit(typeof(object), hex.Arguments.Select(a => Visit(a)))
                };
                if (helper.Target != null)
                {
                    return Expression.Call(
                        Expression.Constant(helper.Target),
#if netstandard
                        helper.GetMethodInfo(),
#else
                        helper.Method,
#endif
                        arguments);
                }
                else
                {
                    return Expression.Call(
#if netstandard
                        helper.GetMethodInfo(),
#else
                        helper.Method,
#endif
                        arguments);
                }
            }
            else
            {
                return Expression.Call(
                    Expression.Constant(this),
#if netstandard
                    new Action<BindingContext, string, IEnumerable<object>>(LateBindHelperExpression).GetMethodInfo(),
#else
                    new Action<BindingContext, string, IEnumerable<object>>(LateBindHelperExpression).Method,
#endif
                    CompilationContext.BindingContext,
                    Expression.Constant(hex.HelperName),
                    Expression.NewArrayInit(typeof(object), hex.Arguments));
            }
        }

        private void LateBindHelperExpression(
            BindingContext context,
            string helperName,
            IEnumerable<object> arguments)
        {
            if (CompilationContext.Configuration.Helpers.ContainsKey(helperName))
            {
                var helper = CompilationContext.Configuration.Helpers[helperName];
                helper(context.TextWriter, context.Value, arguments.ToArray());
            }
            else
            {
                throw new HandlebarsRuntimeException(string.Format("Template references a helper that is not registered. Could not find helper '{0}'", helperName));
            }
        }
    }
}
