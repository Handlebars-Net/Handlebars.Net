using System;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class ContextAccessorExpression : HandlebarsExpression
    {
        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.ContextAccessorExpression; }
        }

        public override Type Type
        {
            get { return typeof(BindingContext); }
        }
    }
}

