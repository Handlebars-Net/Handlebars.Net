using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    /// <summary>
    /// Turns a bare <c>{{^}}</c> (also <c>{{^ }}</c>, <c>{{~^~}}</c>) into the same <c>else</c> marker
    /// that <c>{{else}}</c> produces, so block accumulators treat it as the inverse separator.
    /// Runs after <see cref="RawHelperAccumulator"/> so raw block bodies keep their literal text.
    /// </summary>
    internal class InverseSeparatorConverter : TokenConverter
    {
        private static readonly InverseSeparatorConverter Converter = new InverseSeparatorConverter();

        public static IEnumerable<object> Convert(IEnumerable<object> sequence, ICompiledHandlebarsConfiguration configuration)
        {
            if (configuration.Compatibility.LegacyStandaloneCaretBehavior) return sequence;
            return Converter.ConvertTokens(sequence).ToList();
        }

        private InverseSeparatorConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var items = sequence as IList<object> ?? sequence.ToList();
            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                yield return item;

                if (item is StartExpressionToken start
                    && !start.IsRaw
                    && index + 2 < items.Count
                    && items[index + 1] is WordExpressionToken word
                    && word.Value == "^"
                    && items[index + 2] is EndExpressionToken)
                {
                    yield return HandlebarsExpression.Helper("else", false, false, word.Context);
                    index++;
                }
            }
        }
    }
}
