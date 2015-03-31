using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class ConditionalBlockAccumulatorContext : BlockAccumulatorContext
    {
        private readonly HelperExpression _startingNode;
        private Expression _accumulatedExpression;
        private List<Expression> _body = new List<Expression>();

        public ConditionalBlockAccumulatorContext(Expression startingNode)
            : base(startingNode)
        {
            startingNode = UnwrapStatement(startingNode);
            _startingNode = (HelperExpression)startingNode;
        }

        public override void HandleElement(Expression item)
        {
            if (IsElseBlock(item))
            {
                _accumulatedExpression = CreateIfBlock(_startingNode, _body);
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
                if (_accumulatedExpression == null)
                {
                    _accumulatedExpression = CreateIfBlock(_startingNode, _body);
                }
                else
                {
                    _accumulatedExpression = Expression.IfThenElse(
                        ((ConditionalExpression)_accumulatedExpression).Test,
                        ((ConditionalExpression)_accumulatedExpression).IfTrue,
                        Expression.Block(_body));
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

        private bool IsElseBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression && ((HelperExpression)item).HelperName == "else";
        }

        private bool IsClosingNode(Expression item)
        {
            item = UnwrapStatement(item);
            var blockName = _startingNode.HelperName.Replace("#", "");
            return item is PathExpression && ((PathExpression)item).Path == "/" + blockName;
        }

        private static Expression CreateIfBlock(HelperExpression startingNode, IEnumerable<Expression> body)
        {
            var condition = HandlebarsExpression.Boolish(startingNode.Arguments.Single());
            body = UnwrapBlockExpression(body);
            body = body.Concat(new Expression[] { Expression.Empty() });
            if (startingNode.HelperName == "#if")
            {
                return Expression.IfThen(condition, Expression.Block(body));
            }
            else if (startingNode.HelperName == "#unless")
            {
                return Expression.IfThen(Expression.Not(condition), Expression.Block(body));
            }
            else
            {
                throw new HandlebarsCompilerException(string.Format(
                        "Tried to create a conditional expression for '{0}'", startingNode.HelperName));
            }
        }

        private static IEnumerable<Expression> UnwrapBlockExpression(IEnumerable<Expression> body)
        {
            if (body.IsOneOf<Expression, BlockExpression>())
            {
                body = body.OfType<BlockExpression>().First().Expressions;
            }
            return body;
        }
    }
}

