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

        private bool _finalBranchClosed;

        /// <summary>
        /// "{{else if x}}" is resolved into a flat chain of conditions below, so it must not also be
        /// desugared into a nested block by <see cref="BlockAccumulator"/>. Any other named chain
        /// ("{{else unless x}}", "{{else customHelper a b}}") is left to that generic desugaring,
        /// which nests the named block helper inside this block's inverse, as handlebars.js does.
        /// </summary>
        internal override bool HandlesChainedElseInternally(Expression elseItem)
        {
            var helperExpression = (HelperExpression)UnwrapStatement(elseItem);
            using var arguments = helperExpression.Arguments.GetEnumerator();
            if (!arguments.MoveNext()) return true;

            var first = arguments.Current;

            // "{{else name}}" with nothing after the name has always rendered as a plain "{{else}}".
            if (!arguments.MoveNext()) return true;

            return !(first is PathExpression path) || string.Equals(path.Path, "if", StringComparison.OrdinalIgnoreCase);
        }

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
                if (_currentCondition == null)
                {
                    var message = $"'{{{{else}}}}' cannot follow another '{{{{else}}}}' in the same '{BlockName}' block";
                    if (UnwrapStatement(item) is HelperExpression { IsStandaloneCaret: true })
                    {
                        message += $". A bare '{{{{^}}}}' is the inverse separator, equivalent to '{{{{else}}}}'; to restore the previous behavior, where it rendered the current context, set Configuration.Compatibility.{nameof(Compatibility.LegacyStandaloneCaretBehavior)} = true";
                    }
                    throw new HandlebarsCompilerException(message, GetReaderContext(item));
                }

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

        public override bool IsClosingElement(Expression item) => IsClosingNode(item);

        // Runs once the block is complete: either at its own closing tag, or - when the last
        // branch was a desugared "{{else name ...}}" nested block - after that nested block,
        // which consumed the shared closing tag itself.
        private void CloseFinalBranch()
        {
            if (_finalBranchClosed) return;
            _finalBranchClosed = true;

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
        }

        public override Expression? AccumulatedBlock
        {
            get
            {
                CloseFinalBranch();

                ConditionalExpression? singleConditional = null;
                foreach (var condition in _conditionalBlock.AsEnumerable().Reverse())
                {
                    singleConditional = Expression.IfThenElse(
                        condition.Test,
                        condition.IfTrue,
                        (Expression?)singleConditional ?? condition.IfFalse);
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
                   && ((HelperExpression)item).Arguments.Count() >= 2
                   && HandlesChainedElseInternally(item);
        }

        private Expression GetElseIfTestExpression(Expression item)
        {
            item = UnwrapStatement(item);
            var helperExpression = (HelperExpression)item;
            var elseBlockName = helperExpression.Arguments.First() is PathExpression elsePath ? elsePath.Path : BlockName;
            var (value, hashParameters) = UnwrapBoolishHelperArguments(elseBlockName, helperExpression.Arguments.Skip(1), helperExpression.Context);
            return HandlebarsExpression.Boolish(value, hashParameters);
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

