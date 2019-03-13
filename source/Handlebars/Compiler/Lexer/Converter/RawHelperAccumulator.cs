using HandlebarsDotNet.Compiler.Lexer;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandlebarsDotNet.Compiler
{
    internal class RawHelperAccumulator : TokenConverter
    {
        public static IEnumerable<object> Accumulate(IEnumerable<object> sequence)
        {
            return new RawHelperAccumulator().ConvertTokens(sequence).ToList();
        }

        private RawHelperAccumulator() {}

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            var buffer = new StringBuilder();
            var isInRawHelperBody = false;
            string openRawHelperName = null;

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (openRawHelperName == null && item is StartExpressionToken startExpressionTokenItem)
                {
                    yield return item;

                    if (!startExpressionTokenItem.IsRaw)
                    {
                        continue;
                    }

                    item = GetNext(enumerator);
                    if (!(item is HelperExpression helperExpression))
                    {
                        throw new HandlebarsCompilerException("Expected HelperExpression, got " + item);
                    }

                    openRawHelperName = helperExpression.HelperName;
                    yield return item;
                }
                else if (item is StartExpressionToken startExpressionToken)
                {
                    item = GetNext(enumerator);
                    if (item is WordExpressionToken word && word.Value == ("/" + openRawHelperName))
                    {
                        yield return Token.Static(buffer.ToString());
                        yield return startExpressionToken;
                        yield return item;

                        openRawHelperName = null;
                        isInRawHelperBody = false;
                        buffer = new StringBuilder();
                    }
                    else
                    {
                        buffer.Append(Stringify(startExpressionToken));
                        buffer.Append(Stringify(item));
                    }
                }
                else if (openRawHelperName != null)
                {
                    if (!isInRawHelperBody && item is EndExpressionToken endExpression && endExpression.IsRaw)
                    {
                        isInRawHelperBody = true;
                        yield return item;
                    }
                    else if (!isInRawHelperBody)
                    {
                        yield return item;
                    }
                    else
                    {
                        buffer.Append(Stringify(item));
                    }
                }
                else
                {
                    yield return item;
                }
            }

            if (isInRawHelperBody || openRawHelperName != null)
            {
                throw new HandlebarsCompilerException($"Reached end of template before raw block helper expression '{openRawHelperName}' was closed");
            }
        }

        private static string Stringify(object item)
        {
            if (item is Token token)
            {
                return token.Value;
            }

            if (item is HelperExpression helperExpression)
            {
                return helperExpression.HelperName;
            }

            return item.ToString();
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

