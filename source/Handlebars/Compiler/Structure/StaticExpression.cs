using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class StaticExpression : HandlebarsExpression
    {
        public StaticExpression(string value)
        {
            Value = value;
        }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.StaticExpression;

        public override Type Type => typeof(void);

        public string Value { get; }
    }
}

