using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class HashParameterConverter : TokenConverter
    {
        private static readonly HashParameterConverter Converter = new HashParameterConverter();

        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return Converter.ConvertTokens(sequence).ToList();
        }

        private HashParameterConverter() { }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;

                while (item is WordExpressionToken word)
                {
                    item = GetNext(enumerator);
                    if (item is AssignmentToken)
                    {
                        yield return HandlebarsExpression.HashParameterAssignmentExpression(word.Value);
                        item = GetNext(enumerator);
                    }
                    else
                    {
                        yield return word;
                    }
                }

                yield return item;
            }
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

