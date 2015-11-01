using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class LayoutRemover : TokenConverter
    {
        public static IEnumerable<object> Remove(IEnumerable<object> sequence)
        {
            return new LayoutRemover().ConvertTokens(sequence).ToList();
        }

        private LayoutRemover()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is StartExpressionToken)
                {
                    var possibleComment = GetNext(enumerator);
                    var possibleEndExpression = GetNext(enumerator);
                    if (possibleComment is LayoutToken)
                    {
                        if ((possibleEndExpression is EndExpressionToken) == false)
                        {
                            throw new HandlebarsCompilerException("Encountered an unexpected symbol at the end of a comment expression");
                        }
                        continue;
                    }
                    else
                    {
                        yield return item;
                        yield return possibleComment;
                        yield return possibleEndExpression;
                    }
                }
                else
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

