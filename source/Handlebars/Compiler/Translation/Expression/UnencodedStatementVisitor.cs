using System.Linq.Expressions;
using Expressions.Shortcuts;
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
                var writer = CompilationContext.Args.EncodedWriter;
                var suppressEncoding = writer.Property(o => o.SuppressEncoding);
                
                var existingValue = new ExpressionContainer<bool>(Expression.Variable(typeof(bool)));
                
                return Block()
                    .Parameter(existingValue)
                    .Line(existingValue.Assign(suppressEncoding))
                    .Line(suppressEncoding.Assign(true))
                    .Line(sex)
                    .Line(suppressEncoding.Assign(existingValue));
            }

            return sex;
        }
    }
}

