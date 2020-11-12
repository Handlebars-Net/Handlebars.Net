using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal class PartialBlockAccumulatorContext : BlockAccumulatorContext
    {
        private readonly PartialExpression _startingNode;
        private readonly List<Expression> _body = new List<Expression>();

        public sealed override string BlockName { get; protected set; }
        
        public PartialBlockAccumulatorContext(Expression startingNode)
            : base(startingNode)
        {
            _startingNode = ConvertToPartialExpression(UnwrapStatement(startingNode));
        }

        public override void HandleElement(Expression item)
        {
            _body.Add(item);
        }

        public override Expression GetAccumulatedBlock()
        {
            return HandlebarsExpression.Partial(
                _startingNode.PartialName,
                _startingNode.Argument,
                _body.Count > 1 ? Expression.Block(_body) : _body.First());
        }

        public override bool IsClosingElement(Expression item)
        {
            item = UnwrapStatement(item);
            return IsClosingNode(item);
        }

        private bool IsClosingNode(Expression item)
        {
            return item is PathExpression && ((PathExpression)item).Path == "/" + BlockName;
        }

        private PartialExpression ConvertToPartialExpression(Expression expression)
        {
            if (expression is PathExpression pathExpression)
            {
                BlockName = pathExpression.Path.Replace("#>", "");
                return HandlebarsExpression.Partial(Expression.Constant(BlockName));
            }

            if (!(expression is HelperExpression helperExpression))
                throw new HandlebarsCompilerException($"Cannot convert '{expression}' to a partial expression");
            
            BlockName = helperExpression.HelperName.Replace("#>", "");
            var argumentsCount = helperExpression.Arguments.Count();
            if (string.IsNullOrEmpty(BlockName))
            {
                switch (argumentsCount)
                {
                    case 0:
                        throw new HandlebarsCompilerException("Partial expression misses the name", helperExpression.Context);
                    case 1:
                        BlockName = helperExpression.Arguments.First().As<PathExpression>().Path;
                        return HandlebarsExpression.Partial(Expression.Constant(BlockName));
                    case 2:
                        BlockName = helperExpression.Arguments.First().As<PathExpression>().Path;
                        return HandlebarsExpression.Partial(Expression.Constant(BlockName), helperExpression.Arguments.Last());
                    default:
                        throw new HandlebarsCompilerException("Cannot convert a multi-argument helper expression to a partial expression", helperExpression.Context);
                }
            }

            return argumentsCount switch
            {
                0 => HandlebarsExpression.Partial(Expression.Constant(BlockName)),
                1 => HandlebarsExpression.Partial(Expression.Constant(BlockName),
                    helperExpression.Arguments.First()),
                _ => throw new HandlebarsCompilerException("Cannot convert a multi-argument helper expression to a partial expression", helperExpression.Context)
            };
        }
    }
}

