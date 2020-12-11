using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class ConditionalBlockAccumulatorContext : BlockAccumulatorContext
    {
        private enum TestType { Direct, Reverse }
        
        private static readonly HashSet<string> ValidHelperNames = new HashSet<string> { "if", "unless" };
        
        private readonly List<ConditionalExpression> _conditionalBlock = new List<ConditionalExpression>();
        private Expression _currentCondition;
        private List<Expression> _bodyBuffer = new List<Expression>();
        
        public sealed override string BlockName { get; protected set; }

        public ConditionalBlockAccumulatorContext(Expression startingNode)
            : base(startingNode)
        {
            startingNode = UnwrapStatement(startingNode);
            var helperExpression = (HelperExpression)startingNode;
            var testType = helperExpression.HelperName[0] == '#' ? TestType.Direct : TestType.Reverse;
            BlockName = helperExpression.HelperName.Substring(1, helperExpression.HelperName.Length - 1);

            if (!ValidHelperNames.Contains(BlockName))
            {
                throw new HandlebarsCompilerException($"Tried to convert {BlockName} expression to conditional block", helperExpression.Context);
            }

            var argument = HandlebarsExpression.Boolish(helperExpression.Arguments.Single());

            _currentCondition = BlockName switch
            {
                "if" when testType == TestType.Direct => argument,
                "if" when testType == TestType.Reverse => Expression.Not(argument),
                "unless" when testType == TestType.Direct => Expression.Not(argument),
                "unless" when testType == TestType.Reverse => argument,
                _ => throw new HandlebarsCompilerException($"Tried to convert {BlockName} expression to conditional block", helperExpression.Context) 
            };
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

