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
                var innerContext = BlockAccumulatorContext.Create(item, parentItem, _configuration);
                if (innerContext != null)
                {
                    context.HandleElement(AccumulateBlock(item, enumerator, innerContext));
                }
                else if (context.IsClosingElement(item))
                {
                    return context.GetAccumulatedBlock();
                }
                else
                {
                    context.HandleElement(item);
                }
            }
            throw new HandlebarsCompilerException($"Reached end of template before block expression '{context.BlockName}' was closed");
        }
    }
}

