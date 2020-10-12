using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockHelperAccumulatorContext : BlockAccumulatorContext
    {
        private readonly HelperExpression _startingNode;
        private readonly bool _trimBefore;
        private readonly bool _trimAfter;
        private Expression _accumulatedBody;
        private Expression _accumulatedInversion;
        private List<Expression> _body = new List<Expression>();

        public BlockHelperAccumulatorContext(Expression startingNode)
            : base(startingNode)
        {
            if (startingNode is StatementExpression statementExpression)
            {
                _trimBefore = statementExpression.TrimBefore;
                _trimAfter = statementExpression.TrimAfter;
            }
            startingNode = UnwrapStatement(startingNode);
            _startingNode = (HelperExpression)startingNode;
        }

        public string HelperName => _startingNode.HelperName;

        public override void HandleElement(Expression item)
        {
            if (IsInversionBlock(item))
            {
                _accumulatedBody = GetBlockBody();
                _body = new List<Expression>();
            }
            else
            {
                _body.Add(item);
            }
        }

        private bool IsInversionBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression && ((HelperExpression)item).HelperName == "else";
        }

        public override bool IsClosingElement(Expression item)
        {
            item = UnwrapStatement(item);
            return IsClosingNode(item);
        }

        private bool IsClosingNode(Expression item)
        {
            var helperName = _startingNode.HelperName
                .Replace("#", string.Empty)
                .Replace("^", string.Empty)
                .Replace("*", string.Empty);
            return item is PathExpression expression && expression.Path == "/" + helperName;
        }

        public override Expression GetAccumulatedBlock()
        {
            if (_accumulatedBody == null)
            {
                _accumulatedBody = GetBlockBody();
                _accumulatedInversion = Expression.Block(Expression.Empty());
            }
            else if (_accumulatedInversion == null)
            {
                _accumulatedInversion = GetBlockBody();
            }

            var resultExpr = HandlebarsExpression.BlockHelper(
                _startingNode.HelperName,
                _startingNode.Arguments.Where(o => o.NodeType != (ExpressionType)HandlebarsExpressionType.BlockParamsExpression),
                _startingNode.Arguments.OfType<BlockParamsExpression>().SingleOrDefault() ?? BlockParamsExpression.Empty(),
                _accumulatedBody,
                _accumulatedInversion,
                _startingNode.IsRaw);

            if (_startingNode.IsRaw)
            {
                return HandlebarsExpression.Statement(
                    resultExpr,
                    false,
                    _trimBefore,
                    _trimAfter);
            }

            return resultExpr;
        }

        private Expression GetBlockBody()
        {
            return _body.Any() ?
                Expression.Block(_body) :
                Expression.Block(Expression.Empty());
        }
    }
}

