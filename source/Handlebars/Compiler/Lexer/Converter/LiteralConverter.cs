using System;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq.Expressions;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class LiteralConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new LiteralConverter().ConvertTokens(sequence).ToList();
        }

        private LiteralConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            foreach (var item in sequence)
            {
                bool boolValue;
                int intValue;

                object result = item;

                if (item is LiteralExpressionToken literalExpression)
                {
                    result = Expression.Convert(Expression.Constant(literalExpression.Value), typeof(object));

                    if (!literalExpression.IsDelimitedLiteral)
                    {
                        if (int.TryParse(literalExpression.Value, out intValue))
                        {
                            result = Expression.Convert(Expression.Constant(intValue), typeof(object));
                        }
                    }
                }
                else if (item is WordExpressionToken wordExpression
                    && bool.TryParse(wordExpression.Value, out boolValue))
                {
                    result = Expression.Convert(Expression.Constant(boolValue), typeof(object));
                }

                yield return result;
            }
        }
    }
}

