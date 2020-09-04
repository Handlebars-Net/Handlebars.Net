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

        public string Value { get; }
        
    }
}

