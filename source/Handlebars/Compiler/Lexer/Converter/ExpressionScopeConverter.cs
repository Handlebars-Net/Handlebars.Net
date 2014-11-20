using System;
using System.Collections.Generic;
using System.Linq;
using Handlebars.Compiler.Lexer;
using System.Linq.Expressions;

namespace Handlebars.Compiler
{
    internal class ExpressionScopeConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new ExpressionScopeConverter().ConvertTokens(sequence).ToList();
        }

        private ExpressionScopeConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if(item is StartExpressionToken)
                {
					var startExpression = item as StartExpressionToken;
                    item = GetNext(enumerator);
                    if((item is Expression) == false)
                    {
                        throw new HandlebarsCompilerException(
                            string.Format("Token '{0}' could not be converted to an expression", item));
                    }
                    yield return HandlebarsExpression.Statement(
						(Expression)item,
						startExpression.IsEscaped);
                    item = GetNext(enumerator);
                    if((item is EndExpressionToken) == false)
                    {
                        throw new HandlebarsCompilerException("Handlebars statement was not reduced to a single expression");
                    }
					if(((EndExpressionToken)item).IsEscaped != startExpression.IsEscaped)
					{
						throw new HandlebarsCompilerException("Starting and ending handleabars do not match");
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

