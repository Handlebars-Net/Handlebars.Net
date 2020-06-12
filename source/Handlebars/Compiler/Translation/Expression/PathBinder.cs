using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Compiler.Structure.Path;
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
            
            var context = Arg<BindingContext>(CompilationContext.BindingContext);
            var value = Arg<object>(Visit(sex.Body));
            return context.Call(o => o.TextWriter.Write(value));
        }

        protected override Expression VisitPathExpression(PathExpression pex)
        {
            var context = Arg<BindingContext>(CompilationContext.BindingContext);
            var pathInfo = CompilationContext.Configuration.PathInfoStore.GetOrAdd(pex.Path);

            var resolvePath = Call(() => PathResolver.ResolvePath(context, ref pathInfo));

            if (!pathInfo.IsValidHelperLiteral || pathInfo.IsThis) return resolvePath;
            
            var helperName = pathInfo.Segments[0].PathChain[0].TrimmedValue;
            var tryBoundHelper = Call(() =>
                HelperFunctionBinder.TryLateBindHelperExpression(context, helperName, ArrayEx.Empty<object>())
            );

            if (pex.Context == PathExpression.ResolutionContext.Parameter)
            {
                return resolvePath;
            }
            
            return Block()
                .Parameter<ResultHolder>(out var result, tryBoundHelper)
                .Line(Condition()
                    .If(result.Member(o => o.Success))
                    .Then(result.Member(o => o.Value))
                    .Else(resolvePath)
                );
        }
    }
}

