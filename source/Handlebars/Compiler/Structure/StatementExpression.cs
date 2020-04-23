using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class StatementExpression : HandlebarsExpression
    {
        public StatementExpression(Expression body, bool isEscaped, bool trimBefore, bool trimAfter)
        {
            Body = body;
            IsEscaped = isEscaped;
            TrimBefore = trimBefore;
            TrimAfter = trimAfter;
        }

        public Expression Body { get; }

        public bool IsEscaped { get; }

        public bool TrimBefore { get; }

        public bool TrimAfter { get; }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.StatementExpression;

        public override Type Type => Body.Type;
    }
}