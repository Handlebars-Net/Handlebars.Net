using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Handlebars.Compiler
{
    internal class BlockHelperExpression : HelperExpression
    {
        private readonly Expression _body;

        public BlockHelperExpression(
            string helperName,
            IEnumerable<Expression> arguments,
            Expression body)
            : base(helperName, arguments)
        {
            _body = body;
        }

        public Expression Body
        {
            get { return _body; }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.BlockExpression; }
        }
    }
}

