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
        private Expression? _currentCondition;
        private List<Expression> _bodyBuffer = new List<Expression>();

        public sealed override string BlockName { get; protected set; }

        // "else if"/"else unless" is already resolved into a flat chain of conditions below;
        // it must not also be desugared into a nested block by BlockAccumulator. Any other
        // "else name ..." is left to BlockAccumulator.
        internal override bool HandlesChainedElse(Expression item) => IsElseIfBlock(item);

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

            var (value, hashParameters) = UnwrapBoolishHelperArguments(BlockName, helperExpression.Arguments, helperExpression.Context);
            var argument = HandlebarsExpression.Boolish(value, hashParameters);

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
                _conditionalBlock.Add(Expression.IfThen(_currentCondition!, SinglifyExpressions(_bodyBuffer)));
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

        public override bool IsClosingElement(Expression item) => IsClosingNode(item);

        public override Expression? AccumulatedBlock
        {
            get
            {
                // The last branch is still in the buffer here: a chained "{{else name ...}}" desugared
                // by BlockAccumulator ends this block without passing the closing tag to IsClosingElement.
                var conditions = new List<ConditionalExpression>(_conditionalBlock);
                Expression? singleConditional = null;
                if (_currentCondition != null)
                {
                    conditions.Add(Expression.IfThen(_currentCondition, SinglifyExpressions(_bodyBuffer)));
                }
                else
                {
                    singleConditional = SinglifyExpressions(_bodyBuffer);
                }

                foreach (var condition in conditions.AsEnumerable().Reverse())
                {
                    singleConditional = Expression.IfThenElse(
                        condition.Test,
                        condition.IfTrue,
                        singleConditional ?? condition.IfFalse);
                }
                return singleConditional;
            }
        }

        private bool IsElseBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression && ((HelperExpression)item).HelperName == "else";
        }

        private bool IsElseIfBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return IsElseBlock(item)
                   && ((HelperExpression)item).Arguments.FirstOrDefault() is PathExpression elsePath
                   && ValidHelperNames.Contains(elsePath.Path);
        }

        private Expression GetElseIfTestExpression(Expression item)
        {
            item = UnwrapStatement(item);
            var helperExpression = (HelperExpression)item;
            var elseBlockName = ((PathExpression)helperExpression.Arguments.First()).Path;
            var (value, hashParameters) = UnwrapBoolishHelperArguments(elseBlockName, helperExpression.Arguments.Skip(1), helperExpression.Context);
            var argument = HandlebarsExpression.Boolish(value, hashParameters);
            return elseBlockName == "unless" ? Expression.Not(argument) : argument;
        }

        private bool IsClosingNode(Expression item)
        {
            item = UnwrapStatement(item);
            return item is PathExpression expression && expression.Path == "/" + ResolvedClosingName;
        }

        private static Expression SinglifyExpressions(IEnumerable<Expression> expressions)
        {
            if (expressions.IsMultiple())
            {
                return Expression.Block(expressions);
            }

            return expressions.SingleOrDefault() ?? Expression.Empty();
        }

        private static (Expression Value, HashParametersExpression HashParameters) UnwrapBoolishHelperArguments(string blockName, IEnumerable<Expression> arguments, IReaderContext? context)
        {
            var value = arguments.FirstOrDefault()
                ?? throw new HandlebarsCompilerException($"#{blockName} requires exactly one argument", context);
            if (arguments.Count() == 1)
            {
                var blankHashParameters = HandlebarsExpression.HashParametersExpression(new Dictionary<string, Expression>());
                return (value, blankHashParameters);
            }

            if (arguments.Count() == 2 && arguments.Last() is HashParametersExpression hashParameters)
            {
                return (value, hashParameters);
            }

            throw new HandlebarsCompilerException($"#{blockName} requires exactly one argument", context);

        }
    }
}

