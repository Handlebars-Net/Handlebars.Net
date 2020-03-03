using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class PathBinder : HandlebarsExpressionVisitor
    {
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PathBinder(context).Visit(expr);
        }

        private PathBinder(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (!(sex.Body is PathExpression)) return Visit(sex.Body);
            
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);

            return context.Property(o => o.TextWriter)
                .Call(o => o.Write(ExpressionShortcuts.Arg<object>(Visit(sex.Body))));
        }

        protected override Expression VisitPathExpression(PathExpression pex)
        {
            var pathResolver = ExpressionShortcuts.New<PathResolver>();
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            return pathResolver.Call(o => o.ResolvePath(context, pex.PathInfo));
        }
    }
}

