using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Expressions.Shortcuts;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Polyfills;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class PathBinder : HandlebarsExpressionVisitor
    {
        private CompilationContext CompilationContext { get; }

        public PathBinder(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }
        
        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (!(sex.Body is PathExpression)) return Visit(sex.Body);
            
            var writer = Arg<EncodedTextWriter>(CompilationContext.EncodedWriter);
            
            var value = Arg<object>(Visit(sex.Body));
            return writer.Call(o => o.Write(value));
        }

        protected override Expression VisitPathExpression(PathExpression pex)
        {
            var context = Arg<BindingContext>(CompilationContext.BindingContext);
            var configuration = CompilationContext.Configuration;
            var pathInfo = configuration.PathInfoStore.GetOrAdd(pex.Path);

            var resolvePath = Call(() => PathResolver.ResolvePath(context, pathInfo));
            
            if (pex.Context == PathExpression.ResolutionContext.Parameter) return resolvePath;
            if (pathInfo.IsVariable || pathInfo.IsThis) return resolvePath;
            if (!pathInfo.IsValidHelperLiteral && !configuration.Compatibility.RelaxedHelperNaming) return resolvePath;
            
            if (!configuration.Helpers.TryGetValue(pathInfo, out var helper))
            {
                helper = new StrongBox<HelperDescriptorBase>(new LateBindHelperDescriptor(pathInfo, configuration));
                configuration.Helpers.Add(pathInfo, helper);
            }
            else if (configuration.Compatibility.RelaxedHelperNaming)
            {
                pathInfo.TagComparer();
                if (!configuration.Helpers.ContainsKey(pathInfo))
                {
                    helper = new StrongBox<HelperDescriptorBase>(new LateBindHelperDescriptor(pathInfo, configuration));
                    configuration.Helpers.Add(pathInfo, helper);
                }
            }

            var argumentsArg = Arg(Arguments.Empty);
            return context.Call(o => helper.Value.ReturnInvoke(o, o.Value, argumentsArg));
        }
    }
}

