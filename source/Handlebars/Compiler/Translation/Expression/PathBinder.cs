using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
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

            var writer = CompilationContext.Args.EncodedWriter;
            
            var value = Arg<object>(Visit(sex.Body));
            return writer.Call(o => o.Write<object>(value));
        }

        protected override Expression VisitPathExpression(PathExpression pex)
        {
            var bindingContext = CompilationContext.Args.BindingContext;
            var configuration = CompilationContext.Configuration;
            var pathInfo = PathInfoStore.Current.GetOrAdd(pex.Path);

            var resolvePath = Call(() => PathResolver.ResolvePath(bindingContext, pathInfo));
            
            if (pex.Context == PathExpression.ResolutionContext.Parameter) return resolvePath;
            if (pathInfo.IsVariable || pathInfo.IsThis) return resolvePath;
            if (!pathInfo.IsValidHelperLiteral && !configuration.Compatibility.RelaxedHelperNaming) return resolvePath;

            var pathInfoLight = new PathInfoLight(pathInfo);
            if (!configuration.Helpers.TryGetValue(pathInfoLight, out var helper))
            {
                // TODO: use IHelperResolver here as well
                var lateBindHelperDescriptor = new LateBindHelperDescriptor(pathInfo);
                helper = new Ref<IHelperDescriptor<HelperOptions>>(lateBindHelperDescriptor);
                configuration.Helpers.AddOrReplace(pathInfoLight, helper);
            }
            else if (helper.Value is LateBindHelperDescriptor existingLateBindHelper
                     && !string.Equals(existingLateBindHelper.Name.Path, pathInfo.Path, System.StringComparison.Ordinal))
            {
                // The helpers dictionary uses case-insensitive keys, so "TEST" and "test" map to the
                // same slot. If the cached LateBindHelperDescriptor was stored for a differently-cased
                // path, create a new one that carries the exact casing of the current expression so
                // that runtime property lookup resolves to the right member.
                var lateBindHelperDescriptor = new LateBindHelperDescriptor(pathInfo);
                helper = new Ref<IHelperDescriptor<HelperOptions>>(lateBindHelperDescriptor);
            }
            else if (configuration.Compatibility.RelaxedHelperNaming)
            {
                pathInfoLight = pathInfoLight.TagComparer();
                if (!configuration.Helpers.ContainsKey(pathInfoLight))
                {
                    var lateBindHelperDescriptor = new LateBindHelperDescriptor(pathInfo);
                    helper = new Ref<IHelperDescriptor<HelperOptions>>(lateBindHelperDescriptor);
                    configuration.Helpers.AddOrReplace(pathInfoLight, helper);
                }
            }

            var options = New(() => new HelperOptions(pathInfo, bindingContext));
            var context = New(() => new Context(bindingContext));
            var argumentsArg = New(() => new Arguments(0));
            return Call(() => helper.Value.Invoke(options, context, argumentsArg));
        }
    }
}

