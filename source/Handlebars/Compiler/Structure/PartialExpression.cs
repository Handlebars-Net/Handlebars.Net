using System;
using HandlebarsDotNet.Compiler;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialExpression : HandlebarsExpression
    {
        private readonly Expression _partialName;
        private readonly Expression _argument;

        public PartialExpression(Expression partialName, Expression argument)
        {
            _partialName = partialName;
            _argument = argument;
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.PartialExpression; }
        }

        public override Type Type
        {
            get { return typeof(void); }
        }

        public Expression PartialName
        {
            get { return _partialName; }
        }

        public Expression Argument
        {
            get { return _argument; }
        }
    }
}

