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

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is StartExpressionToken startExpressionTokenItem)
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

                    yield return item;

                    foreach (var param in CollectParameters(enumerator, helperExpression.HelperName))
                    {
                        yield return param;
                    }
                    foreach (var bodyMember in CollectBody(enumerator, helperExpression.HelperName))
                    {
                        yield return bodyMember;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<object> CollectParameters(IEnumerator<object> enumerator, string rawHelperName)
        {
            var unclosedExpressions = 1;
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is EndExpressionToken)
                {
                    unclosedExpressions--;
                    yield return item;

                    if (unclosedExpressions == 0)
                    {
                        yield break;
                    }
                }
                if (item is StartExpressionToken)
                {
                    unclosedExpressions++;
                    yield return item;
                }
                else
                {
                    yield return item;
                }
            }

            throw new HandlebarsCompilerException($"Reached end of template before raw block helper expression '{rawHelperName}' tag was closed");
        }

        private IEnumerable<object> CollectBody(IEnumerator<object> enumerator, string rawHelperName)
        {
            var buffer = new StringBuilder();

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is StartExpressionToken startExpressionToken)
                {
                    item = GetNext(enumerator);
                    if (IsClosingTag(startExpressionToken, item, rawHelperName))
                    {
                        yield return Token.Static(buffer.ToString());
                        yield return startExpressionToken;
                        yield return item;
                        yield break;
                    }

                    buffer.Append(Stringify(startExpressionToken));
                    buffer.Append(Stringify(item));
                }
                else
                {
                    buffer.Append(Stringify(item));
                }
            }

            throw new HandlebarsCompilerException($"Reached end of template before raw block helper expression '{rawHelperName}' was closed");
        }

        private bool IsClosingTag(StartExpressionToken startExpressionToken, object item, string helperName)
        {
            return startExpressionToken.IsRaw
                && item is WordExpressionToken word
                && word.Value == ("/" + helperName);
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

