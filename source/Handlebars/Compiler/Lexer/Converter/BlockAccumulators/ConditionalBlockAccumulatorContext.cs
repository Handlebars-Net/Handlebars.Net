using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class ConditionalBlockAccumulatorContext : BlockAccumulatorContext
    {
        private static readonly HashSet<string> ValidHelperNames = new HashSet<string> { "if", "unless" };
        
        private readonly List<ConditionalExpression> _conditionalBlock = new List<ConditionalExpression>();
        private Expression _currentCondition;
        private List<Expression> _bodyBuffer = new List<Expression>();
        
        public string BlockName { get; }

        public ConditionalBlockAccumulatorContext(Expression startingNode)
            : base(startingNode)
        {
            startingNode = UnwrapStatement(startingNode);
            BlockName = ((HelperExpression)startingNode).HelperName.Replace("#", "");
            if (!ValidHelperNames.Contains(BlockName))
            {
                throw new HandlebarsCompilerException(string.Format(
                        "Tried to convert {0} expression to conditional block", BlockName));
            }
            var testType = BlockName == "if";
            var argument = HandlebarsExpression.Boolish(((HelperExpression)startingNode).Arguments.Single());
            _currentCondition = testType ? (Expression)argument : Expression.Not(argument);
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
            return item is PathExpression expression && expression.Path == "/" + BlockName;
        }

        private static Expression SinglifyExpressions(IEnumerable<Expression> expressions)
        {
            if (expressions.IsMultiple())
            {
                return Expression.Block(expressions);
            }

            return expressions.SingleOrDefault() ?? Expression.Empty();
        }
    }
}

