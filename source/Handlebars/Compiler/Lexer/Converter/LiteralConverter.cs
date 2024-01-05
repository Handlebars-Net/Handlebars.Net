using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class LiteralConverter : TokenConverter
    {
        private static readonly LiteralConverter Converter = new LiteralConverter();

        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return Converter.ConvertTokens(sequence).ToList();
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
                            if (!literalExpression.IsDelimitedLiteral)
                            {
                                if (int.TryParse(literalExpression.Value, out var intValue))
                                {
                                    result = Expression.Convert(Expression.Constant(intValue), typeof(object));
                                }
                                else if (long.TryParse(literalExpression.Value, out var longValue))
                                {
                                    result = Expression.Convert(Expression.Constant(longValue), typeof(object));
                                }
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