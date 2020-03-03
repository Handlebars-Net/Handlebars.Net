using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockHelperExpression : HelperExpression
    {
        public BlockHelperExpression(
            string helperName,
            IEnumerable<Expression> arguments,
            Expression body,
            Expression inversion,
            bool isRaw = false)
            : this(helperName, arguments, BlockParamsExpression.Empty(), body, inversion, isRaw)
        {
        }
        
        public BlockHelperExpression(
            string helperName,
            IEnumerable<Expression> arguments,
            BlockParamsExpression blockParams,
            Expression body,
            Expression inversion,
            bool isRaw = false)
            : base(helperName, arguments, isRaw)
        {
            Body = body;
            Inversion = inversion;
            BlockParams = blockParams;
        }

        public Expression Body { get; }

        public Expression Inversion { get; }

        public BlockParamsExpression BlockParams { get; }

        public override ExpressionType NodeType => (ExpressionType) HandlebarsExpressionType.BlockExpression;
    }
}

