using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
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
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                var startExpression = item as StartExpressionToken;

                if (startExpression == null)
                {
                    yield return item;
                    continue;
                }

                var possibleBody = GetNext(enumerator);
                if (!(possibleBody is Expression))
                {
                    throw new HandlebarsCompilerException($"Token '{possibleBody}' could not be converted to an expression");
                }

                if (!(GetNext(enumerator) is EndExpressionToken endExpression))
                {
                    throw new HandlebarsCompilerException("Handlebars statement was not reduced to a single expression");
                }

                if (endExpression.IsEscaped != startExpression.IsEscaped)
                {
                    throw new HandlebarsCompilerException("Starting and ending handlebars do not match", endExpression.Context);
                }

                yield return HandlebarsExpression.Statement(
                    (Expression) possibleBody,
                    startExpression.IsEscaped,
                    startExpression.TrimPreceedingWhitespace,
                    endExpression.TrimTrailingWhitespace);
            }
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}