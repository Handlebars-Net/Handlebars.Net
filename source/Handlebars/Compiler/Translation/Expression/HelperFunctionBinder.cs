using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

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
            var arguments = new Expression[]
            {
                Expression.Property(
                    CompilationContext.BindingContext,
#if netstandard
                    typeof(BindingContext).GetRuntimeProperty("TextWriter")
#else
                    typeof(BindingContext).GetProperty("TextWriter")
#endif
                ),
                Expression.Property(
                    CompilationContext.BindingContext,
#if netstandard
                    typeof(BindingContext).GetRuntimeProperty("Value")
#else
                    typeof(BindingContext).GetProperty("Value")
#endif
                ),
                Expression.Constant(hex.HelperName),
                Expression.NewArrayInit(typeof(object), hex.Arguments.Select(a => Visit(a)))
            };

            if (CompilationContext.Configuration.Helpers.ContainsKey(hex.HelperName))
            {
                var helper = GetHelperWithName(CompilationContext.Configuration.Helpers[hex.HelperName]);
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
                    new HandlebarsHelperWithName(InvokeLateBindHelper).GetMethodInfo(),
#else
                    new HandlebarsHelperWithName(InvokeLateBindHelper).Method,
#endif
                    arguments);
            }
        }

        private HandlebarsHelperWithName GetHelperWithName(HandlebarsHelper helper)
            => (writer, context, name, arguments) => helper(writer, context, arguments);

        private void InvokeLateBindHelper(
            TextWriter writer,
            dynamic bindingContext,
            string helperName,
            IEnumerable<object> arguments)
        {
            if (!TryInvokeLateBoundHelper(CompilationContext, writer, bindingContext, helperName, arguments))
                throw new HandlebarsRuntimeException(string.Format(
                    "Template references a helper that is not registered. Could not find helper '{0}'", helperName));
        }

        public static bool TryInvokeLateBoundHelper(
            CompilationContext compilationContext,
            TextWriter writer,
            dynamic bindingContext,
            string helperName,
            IEnumerable<object> arguments)
        {
            if (compilationContext.Configuration.Helpers.ContainsKey(helperName))
            {
                var helper = compilationContext.Configuration.Helpers[helperName];
                helper(writer, bindingContext, arguments.ToArray());
                return true;
            }

            return false;
        }
    }
}
