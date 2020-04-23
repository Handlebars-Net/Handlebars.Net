using System.Linq.Expressions;
using Expressions.Shortcuts;

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
            var context = ExpressionShortcuts.Arg<BindingContext>(CompilationContext.BindingContext);
            var suppressEncoding = context.Property(o => o.SuppressEncoding);
            if (sex.IsEscaped == false)
            {
                return ExpressionShortcuts.Block(typeof(void))
                    .Line(suppressEncoding.Assign(true))
                    .Line(sex)
                    .Line(suppressEncoding.Assign(false))
                    .Line(Expression.Empty());
            }

            return sex;
        }
    }
}

