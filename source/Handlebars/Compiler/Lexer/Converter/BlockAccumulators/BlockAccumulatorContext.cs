using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.StringUtils;

namespace HandlebarsDotNet.Compiler
{
    internal abstract class BlockAccumulatorContext
    {
        private static readonly HashSet<string> ConditionHelpers = new HashSet<string>(StringComparer.OrdinalIgnoreCase){ "#if", "#unless", "^if", "^unless" };
        private static readonly HashSet<string> IteratorHelpers = new HashSet<string>(StringComparer.OrdinalIgnoreCase){ "#each", "^each" };

        public static BlockAccumulatorContext Create(Expression item, Expression parentItem, ICompiledHandlebarsConfiguration configuration)
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
            else if (IsDetachedClosingElement(item, parentItem, out var closingElement))
            {
                throw new HandlebarsCompilerException($"A closing element '{closingElement}' was found without a matching open element");
            }

            return context;
        }

        public abstract string BlockName { get; protected set; }

        private static bool IsConditionalBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression helperExpression 
                   && ConditionHelpers.Contains(helperExpression.HelperName);
        }

        private static bool IsBlockHelper(Expression item, ICompiledHandlebarsConfiguration configuration)
        {
            item = UnwrapStatement(item);
            if (item is HelperExpression hitem)
            {
                var helperName = hitem.HelperName;
                var helperPathInfo = PathInfo.Parse(helperName);
                return hitem.IsBlock || !configuration.Helpers.ContainsKey(helperPathInfo) && configuration.BlockHelpers.ContainsKey(helperPathInfo);
            }
            return false;
        }

        private static bool IsIteratorBlock(Expression item)
        {
            item = UnwrapStatement(item);
            return item is HelperExpression expression 
                   && IteratorHelpers.Contains(expression.HelperName);
        }

        private static bool IsPartialBlock (Expression item)
        {
            item = UnwrapStatement (item);
            return item switch
            {
                PathExpression expression => expression.Path.StartsWith("#>"),
                HelperExpression helperExpression => helperExpression.HelperName.StartsWith("#>"),
                _ => false,
            };
        }

        private static bool IsDetachedClosingElement(Expression item, Expression parentItem, out string closingElement)
        {
            closingElement = null;

            var itemElement = GetItemElement(item);

            if (itemElement == null) return false;

            var parentItemElement = GetItemElement(parentItem);

            if (!itemElement.StartsWith("/")) return false;

            if (parentItemElement == null || IsClosingElementNotMatchOpenElement(itemElement, parentItemElement))
            {
                closingElement = itemElement;

                return true;
            }

            return false;
        }

        private static bool IsClosingElementNotMatchOpenElement(string closingElement, string openElement)
        {
            if (closingElement == null) throw new ArgumentNullException(nameof(closingElement));
            if (openElement == null) throw new ArgumentNullException(nameof(openElement));

            if (!openElement.StartsWith("#") || openElement.StartsWith("#>") || openElement.StartsWith("#*")) return false;

            return new Substring(openElement, 1) != new Substring(closingElement, 1);
        }

        private static string GetItemElement(Expression item)
        {
            item = UnwrapStatement(item);
            return item switch
            {
                PathExpression pathExpression => pathExpression.Path,
                HelperExpression helperExpression => helperExpression.HelperName,
                _ => null,
            };
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

