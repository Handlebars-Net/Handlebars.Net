using System.Linq;
using System.Linq.Expressions;
using System.IO;
using Expressions.Shortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperFunctionBinder : HandlebarsExpressionVisitor
    {
        private CompilationContext CompilationContext { get; }

        public HelperFunctionBinder(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            return sex.Body is HelperExpression ? Visit(sex.Body) : sex;
        }
        
        protected override Expression VisitHelperExpression(HelperExpression hex)
        {
            var helperName = hex.HelperName;
            var bindingContext = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var textWriter = bindingContext.Property(o => o.TextWriter);
            var arguments = hex.Arguments.Select(o => FunctionBuilder.Reduce(o, CompilationContext));
            var args = ExpressionShortcuts.Array<object>(arguments);

            var configuration = CompilationContext.Configuration;
            if (configuration.Helpers.TryGetValue(helperName, out var helper))
            {
                return ExpressionShortcuts.Call(() => helper(textWriter, bindingContext, args));
            }
            
            if (configuration.ReturnHelpers.TryGetValue(helperName, out var returnHelper))
            {
                return ExpressionShortcuts.Call(() =>
                    CaptureResult(textWriter, ExpressionShortcuts.Call(() => returnHelper(bindingContext, args)))
                );
            }

            var pureHelperName = helperName.Substring(1);
            foreach (var resolver in configuration.HelperResolvers)
            {
                if (resolver.TryResolveReturnHelper(pureHelperName, typeof(object), out var resolvedHelper))
                {
                    return ExpressionShortcuts.Call(() =>
                        CaptureResult(textWriter, ExpressionShortcuts.Call(() => resolvedHelper(bindingContext, args)))
                    );
                }
            }

            return ExpressionShortcuts.Call(() => 
                CaptureResult(textWriter, ExpressionShortcuts.Call(() => 
                    LateBindHelperExpression(bindingContext, helperName, args)
                ))
            );
        }

        private static object LateBindHelperExpression(BindingContext context, string helperName, object[] arguments)
        {
            var configuration = context.Configuration;
            if (configuration.Helpers.TryGetValue(helperName, out var helper))
            {
                using (var write = new PolledStringWriter(configuration.FormatProvider))
                {
                    helper(write, context.Value, arguments);
                    return write.ToString();
                }
            }
            
            if (configuration.ReturnHelpers.TryGetValue(helperName, out var returnHelper))
            {
                return returnHelper(context.Value, arguments);
            }

            var pureHelperName = helperName.Substring(1);
            foreach (var resolver in configuration.HelperResolvers)
            {
                if (resolver.TryResolveReturnHelper(pureHelperName, arguments.FirstOrDefault()?.GetType(), out returnHelper))
                {
                    return returnHelper(context.Value, arguments);
                }
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
