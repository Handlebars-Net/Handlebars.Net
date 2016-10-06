using System;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal abstract class BlockAccumulatorContext
    {
        public static BlockAccumulatorContext Create(Expression item, HandlebarsConfiguration configuration)
        {
            BlockAccumulatorContext context = null;
            if (IsConditionalBlock(item))
            {
                context = new ConditionalBlockAccumulatorContext(item);
            }
            else if (IsPartialBlock(item))
            {
                context = new PartialBlockAccumulatorContext(item);
            }
            else if (IsBlockHelper(item, configuration))
            {
                context = new BlockHelperAccumulatorContext(item);
            }
            else if (IsIteratorBlock(item))
            {
                context = new IteratorBlockAccumulatorContext(item);
            }
            else if (IsDeferredBlock(item))
            {
                context = new DeferredBlockAccumulatorContext(item);
            }
            return context;
        }

        private static bool IsConditionalBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return (item is HelperExpression) && new[] { "#if", "#unless" }.Contains(((HelperExpression)item).HelperName);
        }

        private static bool IsBlockHelper(Expression item, HandlebarsConfiguration configuration)
        {
            item = UnwrapStatement(item);
            return (item is HelperExpression) && configuration.BlockHelpers.ContainsKey(((HelperExpression)item).HelperName.Replace("#", ""));
        }

        private static bool IsIteratorBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return (item is HelperExpression) && new[] { "#each" }.Contains(((HelperExpression)item).HelperName);
        }

        private static bool IsDeferredBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return (item is PathExpression) && (((PathExpression)item).Path.StartsWith("#") || ((PathExpression)item).Path.StartsWith("^"));
        }

        private static bool IsPartialBlock (Expression item)
        {
            item = UnwrapStatement (item);
            if (item is PathExpression)
            {
                return ((PathExpression)item).Path.StartsWith("#>");
            }
            else if (item is HelperExpression)
            {
                return ((HelperExpression)item).HelperName.StartsWith("#>");
            }
            else
            {
                return false;
            }
        }

        protected static Expression UnwrapStatement(Expression item)
        {
            if (item is StatementExpression)
            {
                return ((StatementExpression)item).Body;
            }
            else
            {
                return item;
            }
        }

        protected BlockAccumulatorContext(Expression startingNode)
        {
        }

        public abstract void HandleElement(Expression item);

        public abstract bool IsClosingElement(Expression item);

        public abstract Expression GetAccumulatedBlock();
    }
}

