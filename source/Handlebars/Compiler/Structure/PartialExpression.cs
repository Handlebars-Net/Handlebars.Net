using System;
using Handlebars.Compiler;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class PartialExpression : HandlebarsExpression
    {
        private readonly string _partialName;

        public PartialExpression(string partialName)
        {
            _partialName = partialName;
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
    }
}

