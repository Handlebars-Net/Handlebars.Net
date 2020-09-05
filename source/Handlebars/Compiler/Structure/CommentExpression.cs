using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class CommentExpression : HandlebarsExpression
    {
        public CommentExpression(string value)
        {
            Value = value;
        }
        
        public string Value { get; }

        public override ExpressionType NodeType => (ExpressionType) HandlebarsExpressionType.CommentExpression;
    }
}