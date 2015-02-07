using System;
using Handlebars.Compiler;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class PartialExpression : HandlebarsExpression
    {
        private readonly string _partialName;
        private readonly Expression _argument;

        public PartialExpression(string partialName, Expression argument)
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

        public string PartialName
        {
            get { return _partialName; }
        }

        public Expression Argument
        {
            get { return _argument; }
        }
    }
}

