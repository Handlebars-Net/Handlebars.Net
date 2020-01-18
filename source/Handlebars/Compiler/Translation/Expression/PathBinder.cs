using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class PathBinder : HandlebarsExpressionVisitor
    {
        private readonly PathResolver _pathResolver;
        
        public static Expression Bind(Expression expr, CompilationContext context)
        {
            return new PathBinder(context).Visit(expr);
        }

        private PathBinder(CompilationContext context)
            : base(context)
        {
            _pathResolver = new PathResolver(context.Configuration);
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (!(sex.Body is PathExpression)) return Visit(sex.Body);
            
            var context = E.Arg<BindingContext>(CompilationContext.BindingContext);

            return context.Property(o => o.TextWriter)
                .Call(o => o.Write(E.Arg<object>(Visit(sex.Body))));
        }

        protected override Expression VisitPathExpression(PathExpression pex)
        {
            var context = E.Arg<BindingContext>(CompilationContext.BindingContext);
            return E.Call(() => _pathResolver.ResolvePath(context, pex.Path));
        }
    }
}

