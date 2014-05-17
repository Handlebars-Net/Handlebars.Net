using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Handlebars.Compiler
{
    internal class BlockHelperAccumulatorContext : BlockAccumulatorContext
	{
        private readonly HelperExpression _startingNode;
        private List<Expression> _body = new List<Expression>();

        public BlockHelperAccumulatorContext(Expression startingNode)
            : base(startingNode)
        {
            startingNode = UnwrapStatement(startingNode);
            _startingNode = (HelperExpression)startingNode;
        }

        public override void HandleElement(Expression item)
        {
            _body.Add(item);
        }

        public override bool IsClosingElement(Expression item)
        {
            item = UnwrapStatement(item);
            var helperName = _startingNode.HelperName.Replace("#", "");
            return item is PathExpression && ((PathExpression)item).Path == "/" + helperName;
        }

        public override Expression GetAccumulatedBlock()
        {
            return HandlebarsExpression.BlockHelper(
                _startingNode.HelperName,
                _startingNode.Arguments,
                Expression.Block(_body));
        }

	}
}

