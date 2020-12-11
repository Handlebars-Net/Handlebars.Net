using System;
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

        public sealed override string BlockName
        {
            get => _startingNode.HelperName;
            protected set => throw new NotSupportedException();
        }

        public override void HandleElement(Expression item)
        {
            if (IsElseBlock(item))
            {
                _accumulatedExpression = HandlebarsExpression.Iterator(BlockName, _startingNode.Arguments.Single(o => o.NodeType != (ExpressionType)HandlebarsExpressionType.BlockParamsExpression), _startingNode.Arguments.OfType<BlockParamsExpression>().SingleOrDefault() ?? BlockParamsExpression.Empty(), Expression.Block(_body));
                _body = new List<Expression>();
            }
            else
            {
                _body.Add(item);
            }
        }

        public override bool IsClosingElement(Expression item)
        {
            if (!IsClosingNode(item)) return false;
            
            // If the template has no content within the block, e.g. `{{#each ...}}{{/each}`, then the block body is a no-op.
            var bodyStatements = _body.Count != 0 ? _body : new List<Expression>{ Expression.Empty() };
            if (_accumulatedExpression == null)
            {
                _accumulatedExpression = HandlebarsExpression.Iterator(BlockName, _startingNode.Arguments.Single(o => o.NodeType != (ExpressionType)HandlebarsExpressionType.BlockParamsExpression), _startingNode.Arguments.OfType<BlockParamsExpression>().SingleOrDefault() ?? BlockParamsExpression.Empty(), Expression.Block(bodyStatements));
            }
            else
            {
                _accumulatedExpression = HandlebarsExpression.Iterator(BlockName, ((IteratorExpression)_accumulatedExpression).Sequence, ((IteratorExpression)_accumulatedExpression).BlockParams, ((IteratorExpression)_accumulatedExpression).Template, Expression.Block(bodyStatements));
            }
            
            return true;
        }

        public override Expression GetAccumulatedBlock()
        {
            return _accumulatedExpression;
        }

        private static bool IsClosingNode(Expression item)
        {
            item = UnwrapStatement(item);
            return item is PathExpression pathExpression && pathExpression.Path.Replace("#", "").Replace("^", "") == "/each";
        }

        private static bool IsElseBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression helperExpression && helperExpression.HelperName == "else";
        }
    }
}

