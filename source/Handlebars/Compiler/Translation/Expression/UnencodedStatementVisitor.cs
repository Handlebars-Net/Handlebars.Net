using System.Linq.Expressions;
using static Expressions.Shortcuts.ExpressionShortcuts;

namespace HandlebarsDotNet.Compiler
{
    internal class UnencodedStatementVisitor : HandlebarsExpressionVisitor
    {
        private CompilationContext CompilationContext { get; }

        public UnencodedStatementVisitor(CompilationContext compilationContext)
        {
            CompilationContext = compilationContext;
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            if (!sex.IsEscaped)
            {
                var context = Arg<BindingContext>(CompilationContext.BindingContext);
                var suppressEncoding = context.Property(o => o.SuppressEncoding);
                
                return Block(typeof(void))
                    .Line(suppressEncoding.Assign(true))
                    .Line(sex)
                    .Line(suppressEncoding.Assign(false))
                    .Line(Expression.Empty());
            }

            return sex;
        }
    }
}

