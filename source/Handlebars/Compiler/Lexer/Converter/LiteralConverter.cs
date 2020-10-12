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
                var result = item;

                switch (item)
                {
                    case LiteralExpressionToken literalExpression:
                    {
                        result = Expression.Convert(Expression.Constant(literalExpression.Value), typeof(object));
                        if (!literalExpression.IsDelimitedLiteral && int.TryParse(literalExpression.Value, out var intValue))
                        {
                            result = Expression.Convert(Expression.Constant(intValue), typeof(object));
                        }
                        
                        break;
                    }

                    case WordExpressionToken wordExpression when bool.TryParse(wordExpression.Value, out var boolValue):
                        result = Expression.Convert(Expression.Constant(boolValue), typeof(object));
                        break;
                }

                yield return result;
            }
        }
    }
}

