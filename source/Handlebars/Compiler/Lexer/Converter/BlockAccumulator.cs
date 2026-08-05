using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockAccumulator : TokenConverter
    {
        public static IEnumerable<object> Accumulate(
            IEnumerable<object> tokens,
            ICompiledHandlebarsConfiguration configuration)
        {
            return new BlockAccumulator(configuration).ConvertTokens(tokens).ToList();
        }

        private readonly ICompiledHandlebarsConfiguration _configuration;

        private BlockAccumulator(ICompiledHandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = (Expression)enumerator.Current;
                var context = BlockAccumulatorContext.Create(item, null, _configuration);
                if (context != null)
                {
                    yield return AccumulateBlock(item, enumerator, context);
                }
                else
                {
                    yield return item;
                }
            }
        }

        private Expression AccumulateBlock(
            Expression parentItem,
            IEnumerator<object> enumerator,
            BlockAccumulatorContext context)
        {
            while (enumerator.MoveNext())
            {
                var item = (Expression)enumerator.Current;

                if (!context.HandlesChainedElseInternally
                    && BlockAccumulatorContext.TryGetChainedElseInvocation(item, out var invocation))
                {
                    return AccumulateChainedElse(item, context, invocation, enumerator);
                }

                var innerContext = BlockAccumulatorContext.Create(item, parentItem, _configuration);
                if (innerContext != null)
                {
                    context.HandleElement(AccumulateBlock(item, enumerator, innerContext));
                }
                else if (context.IsClosingElement(item))
                {
                    return context.AccumulatedBlock;
                }
                else
                {
                    context.HandleElement(item);
                }
            }
            throw new HandlebarsCompilerException($"Reached end of template before block expression '{context.BlockName}' was closed");
        }

        /// <summary>
        /// Desugars "{{else name arg1 arg2}}body{{/outer}}" into the equivalent of
        /// "{{else}}{{#name arg1 arg2}}body{{/name}}{{/outer}}", except the nested "{{#name}}"
        /// block shares the outer block's closing tag instead of requiring its own - the outer
        /// closing tag is what ultimately terminates the recursive accumulation below, so this
        /// method always returns the outer block, never loops back into the caller.
        /// </summary>
        private Expression AccumulateChainedElse(
            Expression elseItem,
            BlockAccumulatorContext context,
            HelperExpression invocation,
            IEnumerator<object> enumerator)
        {
            context.HandleElement(elseItem);

            var nestedContext = BlockAccumulatorContext.Create(invocation, elseItem, _configuration)
                ?? throw new HandlebarsCompilerException($"'{invocation.HelperName.Substring(1)}' cannot be used as a chained else block", invocation.Context);

            nestedContext.SetClosingNameOverride(context.ResolvedClosingName);

            // The nested block has no literal closing tag - it is closed by the outer block's
            // closing tag - so the parent reference used for detached-closing-tag detection has
            // to describe that shared closing tag rather than the nested helper's own name.
            var closingTagMarker = HandlebarsExpression.Helper("#" + nestedContext.ResolvedClosingName, true);
            var nestedBlock = AccumulateBlock(closingTagMarker, enumerator, nestedContext);

            context.HandleElement(nestedBlock);
            return context.AccumulatedBlock!;
        }
    }
}

