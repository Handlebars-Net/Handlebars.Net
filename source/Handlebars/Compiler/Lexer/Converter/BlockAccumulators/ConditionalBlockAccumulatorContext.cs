using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class ConditionalBlockAccumulatorContext : BlockAccumulatorContext
    {
        private List<ConditionalExpression> _conditionalBlock = new List<ConditionalExpression>();
        private Expression _currentCondition;
        private List<Expression> _bodyBuffer = new List<Expression>();
        private readonly bool _testType;
        private readonly string _blockName;

        public ConditionalBlockAccumulatorContext(Expression startingNode)
            : base(startingNode)
        {
            startingNode = UnwrapStatement(startingNode);
            _blockName = ((HelperExpression)startingNode).HelperName.Replace("#", "");
            if (new [] { "if", "unless" }.Contains(_blockName) == false)
            {
                throw new HandlebarsCompilerException(string.Format(
                        "Tried to convert {0} expression to conditional block", _blockName));
            }
            _testType = _blockName == "if";
            var argument = HandlebarsExpression.Boolish(((HelperExpression)startingNode).Arguments.Single());
            _currentCondition = _testType ? (Expression)argument : Expression.Not(argument);
        }

        public override void HandleElement(Expression item)
        {
            if (IsElseBlock(item))
            {
                _conditionalBlock.Add(Expression.IfThen(_currentCondition, SinglifyExpressions(_bodyBuffer)));
                if (IsElseIfBlock(item))
                {
                    _currentCondition = GetElseIfTestExpression(item);
                }
                else
                {
                    _currentCondition = null;
                }
                _bodyBuffer = new List<Expression>();
            }
            else
            {
                _bodyBuffer.Add((Expression)item);
            }
        }

        public override bool IsClosingElement(Expression item)
        {
            if (IsClosingNode(item))
            {
                if (_currentCondition != null)
                {
                    _conditionalBlock.Add(Expression.IfThen(_currentCondition, SinglifyExpressions(_bodyBuffer)));
                }
                else
                {
                    var lastCondition = _conditionalBlock.Last();
                    _conditionalBlock[_conditionalBlock.Count - 1] = Expression.IfThenElse(
                        lastCondition.Test,
                        lastCondition.IfTrue,
                        SinglifyExpressions(_bodyBuffer));
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
            ConditionalExpression singleConditional = null;
            foreach (var condition in _conditionalBlock.AsEnumerable().Reverse())
            {
                singleConditional = Expression.IfThenElse(
                    condition.Test,
                    condition.IfTrue,
                    (Expression)singleConditional ?? condition.IfFalse);
            }
            return singleConditional;
        }

        private bool IsElseBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression && ((HelperExpression)item).HelperName == "else";
        }

        private bool IsElseIfBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return IsElseBlock(item) && ((HelperExpression)item).Arguments.Count() == 2;
        }

        private Expression GetElseIfTestExpression(Expression item)
        {
            item = UnwrapStatement(item);
            return HandlebarsExpression.Boolish(((HelperExpression)item).Arguments.Skip(1).Single());
        }

        private bool IsClosingNode(Expression item)
        {
            item = UnwrapStatement(item);
            return item is PathExpression && ((PathExpression)item).Path == "/" + _blockName;
        }

        private static IEnumerable<Expression> UnwrapBlockExpression(IEnumerable<Expression> body)
        {
            if (body.IsOneOf<Expression, BlockExpression>())
            {
                body = body.OfType<BlockExpression>().First().Expressions;
            }
            return body;
        }

        private static Expression SinglifyExpressions(IEnumerable<Expression> expressions)
        {
            if (expressions.Count() > 1)
            {
                return Expression.Block(expressions);
            }
            else
            {
                return expressions.Single();
            }
        }
    }
}

