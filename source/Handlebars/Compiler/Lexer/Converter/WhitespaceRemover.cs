using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class WhitespaceRemover : TokenConverter
    {
        public static IEnumerable<object> Remove(IEnumerable<object> sequence)
        {
            return new WhitespaceRemover().ConvertTokens(sequence).ToList();
        }

        private WhitespaceRemover()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is EndExpressionToken && ((EndExpressionToken)item).TrimTrailingWhitespace)
                {
                    var endToken = (EndExpressionToken)item;
                    yield return endToken;
                    item = GetNext(enumerator);
                    if (item != null)
                    {
                        if (item is StaticToken)
                        {
                            item = Token.Static(((StaticToken)item).Value.TrimStart());
                        }
                    }
                }
                if (item is StaticToken)
                {
                    var staticToken = (StaticToken)item;
                    item = GetNext(enumerator);
                    if (item != null)
                    {
                        if (item is StartExpressionToken && ((StartExpressionToken)item).TrimPreceedingWhitespace)
                        {
                            staticToken = Token.Static(staticToken.Value.TrimEnd());
                        }
                    }
                    yield return staticToken;
                    if (item == staticToken)
                    {
                        continue;
                    }
                }
                if (item != null)
                {
                    yield return item;
                }
            }
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

