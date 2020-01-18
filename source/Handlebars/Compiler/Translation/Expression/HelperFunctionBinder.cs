using System.Linq;
using System.Linq.Expressions;
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
            return sex.Body is HelperExpression ? Visit(sex.Body) : sex;
        }
        
        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            var bindingContext = E.Arg<BindingContext>(CompilationContext.BindingContext);
            var textWriter = bindingContext.Property(o => o.TextWriter);
            var args = E.Array<object>(hex.Arguments.Select(Visit));

            if (CompilationContext.Configuration.Helpers.TryGetValue(hex.HelperName, out var helper))
            {
                return E.Call(() => helper(textWriter, bindingContext, args));
            }
            
            if (CompilationContext.Configuration.ReturnHelpers.TryGetValue(hex.HelperName, out var returnHelper))
            {
                return E.Call(() => CaptureResult(textWriter, 
                    E.Call(() => returnHelper(bindingContext, args)))
                );
            }

            return E.Call(() => CaptureResult(bindingContext.Property(o => o.TextWriter),
                E.Call(() => LateBindHelperExpression(bindingContext, hex.HelperName, args)))
            );
        }

        private object LateBindHelperExpression(
            BindingContext context,
            string helperName,
            object[] arguments)
        {
            if (CompilationContext.Configuration.Helpers.TryGetValue(helperName, out var helper))
            {
                using (var write = new StringWriter())
                {
                    helper(write, context.Value, arguments.ToArray());
                    return write.ToString();
                }
            }
            
            if (CompilationContext.Configuration.ReturnHelpers.TryGetValue(helperName, out var returnHelper))
            {
                return returnHelper(context.Value, arguments.ToArray());
            }

            throw new HandlebarsRuntimeException($"Template references a helper that is not registered. Could not find helper '{helperName}'");
        }
        
        private static object CaptureResult(TextWriter writer, object result)
        {
            writer?.WriteSafeString(result);
            return result;
        }
    }
}
