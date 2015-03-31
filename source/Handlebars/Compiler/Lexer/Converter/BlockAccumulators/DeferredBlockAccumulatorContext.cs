using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal class DeferredBlockAccumulatorContext : BlockAccumulatorContext
    {
        private readonly PathExpression _startingNode;
        private List<Expression> _body = new List<Expression>();
        private Expression _accumulatedExpression;

        public DeferredBlockAccumulatorContext(Expression startingNode)
            : base(startingNode)
        {
            startingNode = UnwrapStatement(startingNode);
            _startingNode = (PathExpression)startingNode;
        }

        public override Expression GetAccumulatedBlock()
        {
            return _accumulatedExpression;
        }

        public override void HandleElement(Expression item)
        {
            _body.Add((Expression)item);
        }

        public override bool IsClosingElement(Expression item)
        {
            if (IsClosingNode(item))
            {
                var evalMode = _startingNode.Path.StartsWith("#")
                    ? SectionEvaluationMode.NonEmpty : SectionEvaluationMode.Empty;
                _accumulatedExpression = HandlebarsExpression.DeferredSection(
                    _startingNode,
                    _body,
                    evalMode);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsClosingNode(Expression item)
        {
            item = UnwrapStatement(item);
            var blockName = _startingNode.Path.Replace("#", "").Replace("^", "");
            return item is PathExpression && ((PathExpression)item).Path == "/" + blockName;
        }
    }
}

