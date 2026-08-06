using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.StringUtils;

namespace HandlebarsDotNet.Compiler
{
    internal abstract class BlockAccumulatorContext
    {
        private static readonly HashSet<string> ConditionHelpers = new HashSet<string>(StringComparer.OrdinalIgnoreCase){ "#if", "#unless", "^if", "^unless" };
        private static readonly HashSet<string> IteratorHelpers = new HashSet<string>(StringComparer.OrdinalIgnoreCase){ "#each", "^each" };

        public static BlockAccumulatorContext? Create(Expression item, Expression? parentItem, ICompiledHandlebarsConfiguration configuration)
        {
            BlockAccumulatorContext? context = null;
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

        /// <summary>
        /// True when this context resolves its own "{{else name ...}}" chaining internally
        /// (e.g. the if/unless flattened else-if chain) instead of relying on the generic
        /// nested-block desugaring performed by <see cref="BlockAccumulator"/>.
        /// </summary>
        internal virtual bool HandlesChainedElseInternally => false;

        private string? _closingNameOverride;

        /// <summary>
        /// Used when this context represents the implicit nested block produced by desugaring
        /// "{{else name ...}}" - it has no closing tag of its own, so it must be closed by
        /// whichever closing tag actually closes the outermost block in the else-chain.
        /// </summary>
        internal void SetClosingNameOverride(string closingName)
        {
            _closingNameOverride = closingName;
        }

        internal string ResolvedClosingName => _closingNameOverride ?? OwnClosingName;

        protected virtual string OwnClosingName => BlockName;

        /// <summary>
        /// Recognizes "{{else name arg1 arg2 hash=val}}" and, if found, builds the synthetic
        /// "{{#name arg1 arg2 hash=val}}" opening node it desugars to.
        /// </summary>
        internal static bool TryGetChainedElseInvocation(Expression item, [NotNullWhen(true)] out HelperExpression? invocation)
        {
            item = UnwrapStatement(item);
            if (item is HelperExpression { HelperName: "else" } helperExpression
                && helperExpression.Arguments.FirstOrDefault() is PathExpression nameExpression)
            {
                invocation = new HelperExpression(
                    "#" + nameExpression.Path,
                    isBlock: true,
                    arguments: helperExpression.Arguments.Skip(1),
                    isRaw: helperExpression.IsRaw,
                    context: helperExpression.Context);
                return true;
            }

            invocation = null;
            return false;
        }

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
                return hitem.IsBlock || !configuration.Helpers.ContainsKey(helperPathInfo) && (configuration.BlockHelpers.ContainsKey(helperPathInfo) || configuration.BlockDecorators.ContainsKey(helperPathInfo));
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

        private static bool IsDetachedClosingElement(Expression item, Expression? parentItem, [NotNullWhen(true)] out string? closingElement)
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

        private static string? GetItemElement(Expression? item)
        {
            item = UnwrapStatement(item);
            return item switch
            {
                PathExpression pathExpression => pathExpression.Path,
                HelperExpression helperExpression => helperExpression.HelperName,
                _ => null,
            };
        }

        [return: NotNullIfNotNull(nameof(item))]
        protected static Expression? UnwrapStatement(Expression? item)
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

        [MemberNotNullWhen(true, nameof(AccumulatedBlock))]
        public abstract bool IsClosingElement(Expression item);

        public abstract Expression? AccumulatedBlock { get; }
    }
}

