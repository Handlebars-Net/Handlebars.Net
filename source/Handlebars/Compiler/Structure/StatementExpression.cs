using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class StatementExpression : HandlebarsExpression
    {
        private readonly Expression _body;

        public StatementExpression(Expression body)
        {
            _body = body;
        }

        public Expression Body
        {
            get { return _body; }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.StatementExpression; }
        }
        public override Type Type
        {
            get { return typeof(void); }
        }
    }
}

