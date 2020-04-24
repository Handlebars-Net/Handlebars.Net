using System;
using HandlebarsDotNet.Compiler;
using System.Linq.Expressions;

namespace HandlebarsDotNet
{
    internal class SubExpressionExpression : HandlebarsExpression
    {
        public SubExpressionExpression(Expression expression)
        {
            Expression = expression;
        }

        public override Type Type => typeof(object);

        public Expression Expression { get; }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.SubExpression;
    }
}

