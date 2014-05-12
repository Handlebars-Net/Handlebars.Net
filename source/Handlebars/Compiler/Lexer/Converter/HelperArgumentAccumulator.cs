using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using Handlebars.Compiler.Lexer;

namespace Handlebars.Compiler
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
            while(enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if(item is HelperExpression)
                {
                    var helper = item as HelperExpression;
                    item = GetNext(enumerator);
                    List<Expression> helperArguments = new List<Expression>();
                    while((item is EndExpressionToken) == false)
                    {
                        if((item is Expression) == false)
                        {
                            throw new HandlebarsCompilerException(
                                string.Format("Token '{0}' could not be converted to an expression", item));
                        }
                        helperArguments.Add((Expression)item);
                        item = GetNext(enumerator);
                    }
                    yield return HandlebarsExpression.Helper(
                        helper.HelperName,
                        helperArguments);
                    yield return item;
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

