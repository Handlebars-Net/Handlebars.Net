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
            if (!(sex.Body is PathExpression pex)) return Visit(sex.Body);

            var configuration = CompilationContext.Configuration;
            var pathInfo = PathInfoStore.Current.GetOrAdd(pex.Path);

            if (pex.Context != PathExpression.ResolutionContext.Parameter
                && !pathInfo.IsVariable
                && !pathInfo.IsThis
                && (pathInfo.IsValidHelperLiteral || configuration.Compatibility.RelaxedHelperNaming))
            {
                var pathInfoLight = new PathInfoLight(pathInfo);
                Ref<IHelperDescriptor<HelperOptions>> helper;

                if (!configuration.Helpers.TryGetValue(pathInfoLight, out helper))
                {
                    var lateBindHelperDescriptor = new LateBindHelperDescriptor(pathInfo);
                    helper = new Ref<IHelperDescriptor<HelperOptions>>(lateBindHelperDescriptor);
                    configuration.Helpers.AddOrReplace(pathInfoLight, helper);
                }
                else if (helper.Value is LateBindHelperDescriptor existingLateBindDescriptor
                         && !string.Equals(existingLateBindDescriptor.Name.Path, pathInfo.Path, System.StringComparison.Ordinal))
                {
                    // The case-insensitive lookup found a late-bind descriptor registered for a
                    // differently-cased path (e.g. {{TEST}} was registered; now compiling {{test}}).
                    // Create a fresh descriptor bound to the exact current path so that runtime
                    // path resolution is case-sensitive.
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

                var bindingContext = CompilationContext.Args.BindingContext;
                var options = New(() => new HelperOptions(pathInfo, bindingContext));
                var contextValue = New(() => new Context(bindingContext));
                var args = New(() => new Arguments(0));
                var textWriter = CompilationContext.Args.EncodedWriter;
                return Call(() => helper.Value.Invoke(textWriter, options, contextValue, args));
            }

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

