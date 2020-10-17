using System;
using System.Linq;
using System.Linq.Expressions;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockParamsExpression : HandlebarsExpression
    {
        public new static BlockParamsExpression Empty() => new BlockParamsExpression(null);

        public readonly BlockParam BlockParam;
        
        private BlockParamsExpression(BlockParam blockParam)
        {
            BlockParam = blockParam;
        }
        
        public BlockParamsExpression(string action, string blockParams)
            :this(new BlockParam
            {
                Action = action,
                Parameters = blockParams.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(ChainSegment.Create)
                    .ToArray()
            })
        {
        }

        public override ExpressionType NodeType { get; } = (ExpressionType)HandlebarsExpressionType.BlockParamsExpression;

        public override Type Type { get; } = typeof(BlockParam);

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.Visit(Constant(BlockParam, typeof(BlockParam)));
        }
    }

    internal class BlockParam
    {
        public string Action { get; set; }
        public ChainSegment[] Parameters { get; set; }
    }
}