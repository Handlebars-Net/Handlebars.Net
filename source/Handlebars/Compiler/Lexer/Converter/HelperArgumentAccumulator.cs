using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperArgumentAccumulator : TokenConverter
    {
        public static IEnumerable<object> Accumulate(IEnumerable<object> sequence)
        {
            return new HelperArgumentAccumulator().ConvertTokens(sequence).ToList();
        }

        private HelperArgumentAccumulator()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                switch (item)
                {
                    case HelperExpression helper:
                    {
                        var helperArguments = AccumulateArguments(enumerator);
                        yield return HandlebarsExpression.Helper(
                            helper.HelperName,
                            helper.IsBlock,
                            helperArguments,
                            helper.IsRaw);
                        yield return enumerator.Current;
                        break;
                    }
                    case PathExpression path:
                    {
                        var helperArguments = AccumulateArguments(enumerator);
                        if (helperArguments.Count > 0)
                        {
                            yield return HandlebarsExpression.Helper(
                                path.Path,
                                false,
                                helperArguments,
                                ((EndExpressionToken) enumerator.Current)?.IsRaw ?? false);
                            yield return enumerator.Current;
                        }
                        else
                        {
                            yield return path;
                            yield return enumerator.Current;
                        }

                        break;
                    }
                    
                    default:
                        yield return item;
                        break;
                }
            }
        }

        private static List<Expression> AccumulateArguments(IEnumerator<object> enumerator)
        {
            var item = GetNext(enumerator);
            var helperArguments = new List<Expression>();
            while (!(item is EndExpressionToken))
            {
                if (!(item is Expression))
                {
                    throw new HandlebarsCompilerException($"Token '{item}' could not be converted to an expression");
                }
                helperArguments.Add((Expression)item);
                item = GetNext(enumerator);
            }
            return helperArguments;
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

