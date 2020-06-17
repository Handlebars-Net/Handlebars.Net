using System.Linq;
using System.Linq.Expressions;
using System.IO;
using Expressions.Shortcuts;
using static Expressions.Shortcuts.ExpressionShortcuts;

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
            var pathInfo = CompilationContext.Configuration.PathInfoStore.GetOrAdd(hex.HelperName);
            if(!pathInfo.IsValidHelperLiteral && !CompilationContext.Configuration.Compatibility.RelaxedHelperNaming) return Expression.Empty();

            var readerContext = Arg(hex.Context);
            var helperName = pathInfo.TrimmedPath;
            var bindingContext = Arg<BindingContext>(CompilationContext.BindingContext);
            var contextValue = bindingContext.Property(o => o.Value);
            var textWriter = bindingContext.Property(o => o.TextWriter);
            var arguments = hex.Arguments
                .ApplyOn<Expression, PathExpression>(path => path.Context = PathExpression.ResolutionContext.Parameter)
                .Select(o => FunctionBuilder.Reduce(o, CompilationContext));
            
            var args = Array<object>(arguments);

            var configuration = CompilationContext.Configuration;
            if (configuration.Helpers.TryGetValue(helperName, out var helper))
            {
                return Call(() => helper(textWriter, contextValue, args));
            }
            
            if (configuration.ReturnHelpers.TryGetValue(helperName, out var returnHelper))
            {
                return Call(() =>
                    CaptureResult(textWriter, Call(() => returnHelper(contextValue, args)))
                );
            }
            
            foreach (var resolver in configuration.HelperResolvers)
            {
                if (resolver.TryResolveReturnHelper(helperName, typeof(object), out var resolvedHelper))
                {
                    return Call(() =>
                        CaptureResult(textWriter, Call(() => resolvedHelper(contextValue, args)))
                    );
                }
            }

            return Call(() => 
                CaptureResult(textWriter, Call(() => 
                    LateBindHelperExpression(bindingContext, helperName, args, (IReaderContext) readerContext)
                ))
            );
        }

        public static ResultHolder TryLateBindHelperExpression(BindingContext context, string helperName, object[] arguments)
        {
            var configuration = context.Configuration;
            if (configuration.Helpers.TryGetValue(helperName, out var helper))
            {
                using (var write = new PolledStringWriter(configuration.FormatProvider))
                {
                    helper(write, context.Value, arguments);
                    var result = write.ToString();
                    return new ResultHolder(true, result);
                }
            }
            
            if (configuration.ReturnHelpers.TryGetValue(helperName, out var returnHelper))
            {
                var result = returnHelper(context.Value, arguments);
                return new ResultHolder(true, result);
            }
            
            var targetType = arguments.FirstOrDefault()?.GetType();
            foreach (var resolver in configuration.HelperResolvers)
            {
                if (!resolver.TryResolveReturnHelper(helperName, targetType, out returnHelper)) continue;
                
                var result = returnHelper(context.Value, arguments);
                return new ResultHolder(true, result);
            }
            
            return new ResultHolder(false, null);
        }
        
        private static object LateBindHelperExpression(BindingContext context, string helperName, object[] arguments,
            IReaderContext readerContext)
        {
            var result = TryLateBindHelperExpression(context, helperName, arguments);
            if (result.Success)
            {
                return result.Value;
            }

            throw new HandlebarsRuntimeException($"Template references a helper that cannot be resolved. Helper '{helperName}'", readerContext);
        }

        private static object CaptureResult(TextWriter writer, object result)
        {
            writer?.WriteSafeString(result);
            return result;
        }
    }
}
