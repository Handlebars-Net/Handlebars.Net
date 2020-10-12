using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal class IteratorBlockAccumulatorContext : BlockAccumulatorContext
    {
        private readonly HelperExpression _startingNode;
        private Expression _accumulatedExpression;
        private List<Expression> _body = new List<Expression>();

        public IteratorBlockAccumulatorContext(Expression startingNode)
            : base(startingNode)
        {
            startingNode = UnwrapStatement(startingNode);
            _startingNode = (HelperExpression)startingNode;
        }

        public string BlockName => _startingNode.HelperName;

        public override void HandleElement(Expression item)
        {
            if (IsElseBlock(item))
            {
                _accumulatedExpression = HandlebarsExpression.Iterator(
                    _startingNode.Arguments.Single(o => o.NodeType != (ExpressionType)HandlebarsExpressionType.BlockParamsExpression),
                    _startingNode.Arguments.OfType<BlockParamsExpression>().SingleOrDefault() ?? BlockParamsExpression.Empty(),
                    Expression.Block(_body));
                _body = new List<Expression>();
            }
            else
            {
                _body.Add((Expression)item);
            }
        }

        public override bool IsClosingElement(Expression item)
        {
            if (IsClosingNode(item))
            {
                // If the template has no content within the block, e.g. `{{#each ...}}{{/each}`, then the block body is a no-op.
                var bodyStatements = _body.Count != 0 ? _body : new List<Expression>{ Expression.Empty() };
                if (_accumulatedExpression == null)
                {
                    _accumulatedExpression = HandlebarsExpression.Iterator(
                        _startingNode.Arguments.Single(o => o.NodeType != (ExpressionType)HandlebarsExpressionType.BlockParamsExpression),
                        _startingNode.Arguments.OfType<BlockParamsExpression>().SingleOrDefault() ?? BlockParamsExpression.Empty(),
                        Expression.Block(bodyStatements));
                }
                else
                {
                    _accumulatedExpression = HandlebarsExpression.Iterator(
                        ((IteratorExpression)_accumulatedExpression).Sequence,
                        ((IteratorExpression)_accumulatedExpression).BlockParams,
                        ((IteratorExpression)_accumulatedExpression).Template,
                        Expression.Block(bodyStatements));
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override Expression GetAccumulatedBlock()
        {
            return _accumulatedExpression;
        }

        private bool IsClosingNode(Expression item)
        {
            item = UnwrapStatement(item);
            return item is PathExpression && ((PathExpression)item).Path.Replace("#", "") == "/each";
        }

        private bool IsElseBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression && ((HelperExpression)item).HelperName == "else";
        }
    }
}

