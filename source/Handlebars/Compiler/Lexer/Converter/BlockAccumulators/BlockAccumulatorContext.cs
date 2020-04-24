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
            else if (IsIteratorBlock(item))
            {
                context = new IteratorBlockAccumulatorContext(item);
            }
            else if (IsBlockHelper(item, configuration))
            {
                context = new BlockHelperAccumulatorContext(item);
            }

            return context;
        }

        public string Name
        {
            get
            {
                var type = GetType();
                if (type == typeof(BlockHelperAccumulatorContext))
                    return ((BlockHelperAccumulatorContext)this).HelperName;

                if (type == typeof(ConditionalBlockAccumulatorContext))
                    return ((ConditionalBlockAccumulatorContext)this).BlockName;

                if (type == typeof(IteratorBlockAccumulatorContext))
                    return ((IteratorBlockAccumulatorContext)this).BlockName;

                return null;
            }
        }

        private static bool IsConditionalBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return (item is HelperExpression) && new[] { "#if", "#unless" }.Contains(((HelperExpression)item).HelperName);
        }

        private static bool IsBlockHelper(Expression item, HandlebarsConfiguration configuration)
        {
            item = UnwrapStatement(item);
            if (item is HelperExpression hitem)
            {
                var helperName = hitem.HelperName;
                return hitem.IsBlock || !(configuration.Helpers.ContainsKey(helperName) || configuration.ReturnHelpers.ContainsKey(helperName)) &&
                       configuration.BlockHelpers.ContainsKey(helperName.Replace("#", "").Replace("^", ""));
            }
            return false;
        }

        private static bool IsIteratorBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression expression && "#each".Equals(expression.HelperName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPartialBlock (Expression item)
        {
            item = UnwrapStatement (item);
            switch (item)
            {
                case PathExpression expression:
                    return expression.Path.StartsWith("#>");
                
                case HelperExpression helperExpression:
                    return helperExpression.HelperName.StartsWith("#>");
                
                default:
                    return false;
            }
        }

        protected static Expression UnwrapStatement(Expression item)
        {
            if (item is StatementExpression expression)
            {
                return expression.Body;
            }

            return item;
        }

        protected BlockAccumulatorContext(Expression startingNode)
        {
        }

        public abstract void HandleElement(Expression item);

        public abstract bool IsClosingElement(Expression item);

        public abstract Expression GetAccumulatedBlock();
    }
}

