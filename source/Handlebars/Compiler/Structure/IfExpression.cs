using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class IfExpression : HandlebarsExpression
    {
        private readonly Expression _predicate;
        private readonly Expression _body;

        public IfExpression(Expression predicate, Expression body)
        {
            _predicate = predicate;
            _body = body;
        }

        public Expression Predicate
        {
            get { return _predicate; }
        }

        public Expression Body
        {
            get { return _body; }
        }

        public override Type Type
        {
            get { return typeof(void); }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.IfExpression; }
        }
    }
}

