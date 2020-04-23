using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class BoolishExpression : HandlebarsExpression
    {
        public BoolishExpression(Expression condition)
        {
            Condition = condition;
        }

        public new Expression Condition { get; }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.BoolishExpression;

        public override Type Type => typeof(bool);
    }
}

