using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Expressions.Shortcuts;
using HandlebarsDotNet.Helpers;
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
            
            var helperName = pathInfo.TrimmedPath;
            var bindingContext = CompilationContext.Args.BindingContext;
            var textWriter = CompilationContext.Args.EncodedWriter;
            
            var contextValue = bindingContext.Property(o => o.Value);
            var args = FunctionBinderHelpers.CreateArguments(hex.Arguments, CompilationContext);

            var configuration = CompilationContext.Configuration;
            if (configuration.Helpers.TryGetValue(pathInfo, out var helper))
            {
                return Call(() => helper.Value.WriteInvoke(bindingContext, textWriter, contextValue, args));
            }
            
            for (var index = 0; index < configuration.HelperResolvers.Count; index++)
            {
                var resolver = configuration.HelperResolvers[index];
                if (resolver.TryResolveHelper(helperName, typeof(object), out var resolvedHelper))
                {
                    helper = new StrongBox<HelperDescriptorBase>(resolvedHelper);
                    configuration.Helpers.Add(pathInfo, helper);
                    return Call(() => resolvedHelper.WriteInvoke(bindingContext, textWriter, contextValue, args));
                }
            }

            var lateBindDescriptor = new StrongBox<HelperDescriptorBase>(new LateBindHelperDescriptor(pathInfo, configuration));
            configuration.Helpers.Add(pathInfo, lateBindDescriptor);
            
            return Call(() => lateBindDescriptor.Value.WriteInvoke(bindingContext, textWriter, contextValue, args));
        }
    }
}
