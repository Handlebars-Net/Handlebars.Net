using System.Linq.Expressions;
using Expressions.Shortcuts;
using HandlebarsDotNet.Compiler.Structure.Path;

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
            
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var value = ExpressionShortcuts.Arg<object>(Visit(sex.Body));
            return context.Call(o => o.TextWriter.Write(value));
        }

        protected override Expression VisitPathExpression(PathExpression pex)
        {
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var pathInfo = pex.PathInfo;

            return ExpressionShortcuts.Call(() => PathResolver.ResolvePath(context, ref pathInfo));
        }
    }
}

