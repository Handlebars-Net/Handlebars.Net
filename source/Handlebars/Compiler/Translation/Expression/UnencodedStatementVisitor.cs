using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class UnencodedStatementVisitor : HandlebarsExpressionVisitor
    {
        public static Expression Visit(Expression expr, CompilationContext context)
        {
            return new UnencodedStatementVisitor(context).Visit(expr);
        }

        private UnencodedStatementVisitor(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitStatementExpression(StatementExpression sex)
        {
            var context = ExpressionShortcuts.Var<BindingContext>();
            var suppressEncoding = context.Property(o => o.SuppressEncoding);
            if (sex.IsEscaped == false)
            {
                return ExpressionShortcuts.Block(typeof(void))
                    .Parameter(context, CompilationContext.BindingContext)
                    .Line(suppressEncoding.Assign(true))
                    .Line(sex)
                    .Line(suppressEncoding.Assign(false))
                    .Line(Expression.Empty());
            }

            return sex;
        }
    }
}

