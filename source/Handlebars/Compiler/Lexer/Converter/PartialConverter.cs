using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using Handlebars.Compiler.Lexer;

namespace Handlebars.Compiler
{
    internal class PartialConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new PartialConverter().ConvertTokens(sequence).ToList();
        }

        private PartialConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is PartialToken)
                {
                    var partialName = GetNext(enumerator) as WordExpressionToken;
                    if (partialName == null)
                    {
                        throw new HandlebarsParserException("Partial indicator not followed by a parseable partial name");
                    }

                    var tempVariable = GetNext(enumerator);
                    EndExpressionToken endExpression = null;
                    var model = tempVariable as WordExpressionToken;
                    if (model == null)
                    {
                        endExpression = tempVariable as EndExpressionToken;
                    }
                    else
                    {

                        endExpression = GetNext(enumerator) as EndExpressionToken;
                        if (endExpression == null)
                        {
                            throw new HandlebarsParserException("Partial reference followed by unexpected token");
                        }
                    }

                    if (model != null)
                    {
                        yield return HandlebarsExpression.Partial(partialName.Value, model.Value);
                    }
                    else
                    {
                        yield return HandlebarsExpression.Partial(partialName.Value, "");
                    }

                    yield return endExpression;
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

