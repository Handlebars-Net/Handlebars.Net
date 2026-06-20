using System;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class BoolishExpression : HandlebarsExpression
    {
        public BoolishExpression(Expression condition, HashParametersExpression hashParameters)
        {
            Condition = condition;
            HashParameters = hashParameters;
        }

        public new Expression Condition { get; }

        public HashParametersExpression HashParameters { get; set; }

        public override ExpressionType NodeType => (ExpressionType)HandlebarsExpressionType.BoolishExpression;

        public override Type Type => typeof(bool);
    }
}

