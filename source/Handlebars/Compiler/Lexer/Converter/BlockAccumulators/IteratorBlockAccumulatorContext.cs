using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.Compiler
{
    internal class IteratorBlockAccumulatorContext : BlockAccumulatorContext
    {
        private readonly HelperExpression _startingNode;
        private Expression? _accumulatedExpression;
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
            [DoesNotReturn]
            protected set => throw new NotSupportedException();
        }

        protected override string OwnClosingName => "each";

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

        public override bool IsClosingElement(Expression item) => IsClosingNode(item);

        public override Expression? AccumulatedBlock
        {
            get
            {
                // If the template has no content within the block, e.g. `{{#each ...}}{{/each}`, then the block body is a no-op.
                var bodyStatements = _body.Count != 0 ? _body : new List<Expression>{ Expression.Empty() };
                if (_accumulatedExpression == null)
                {
                    return HandlebarsExpression.Iterator(BlockName, _startingNode.Arguments.Single(o => o.NodeType != (ExpressionType)HandlebarsExpressionType.BlockParamsExpression), _startingNode.Arguments.OfType<BlockParamsExpression>().SingleOrDefault() ?? BlockParamsExpression.Empty(), Expression.Block(bodyStatements));
                }

                return HandlebarsExpression.Iterator(BlockName, ((IteratorExpression)_accumulatedExpression).Sequence, ((IteratorExpression)_accumulatedExpression).BlockParams, ((IteratorExpression)_accumulatedExpression).Template, Expression.Block(bodyStatements));
            }
        }

        private bool IsClosingNode(Expression item)
        {
            item = UnwrapStatement(item);
            return item is PathExpression pathExpression && pathExpression.Path.Replace("#", "").Replace("^", "") == "/" + ResolvedClosingName;
        }

        private static bool IsElseBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression helperExpression && helperExpression.HelperName == "else";
        }
    }
}

