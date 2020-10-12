using System.Linq.Expressions;
using System.Collections.Generic;

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
            : base(helperName, true, arguments, isRaw)
        {
            Body = body;
            Inversion = inversion;
            BlockParams = blockParams;
            IsBlock = true;
        }

        public Expression Body { get; }

        public Expression Inversion { get; }

        public new BlockParamsExpression BlockParams { get; }

        public override ExpressionType NodeType => (ExpressionType) HandlebarsExpressionType.BlockExpression;
    }
}

