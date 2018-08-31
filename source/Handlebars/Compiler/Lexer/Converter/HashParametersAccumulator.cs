using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class HashParametersAccumulator : TokenConverter
    {
        public static IEnumerable<object> Accumulate(IEnumerable<object> sequence)
        {
            return new HashParametersAccumulator().ConvertTokens(sequence).ToList();
        }

        private HashParametersAccumulator() { }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;

                if (item is HashParameterAssignmentExpression parameterAssignment)
                {
                    bool moveNext;
                    var parameters = AccumulateParameters(enumerator, out moveNext);

                    if (parameters.Any())
                    {
                        yield return HandlebarsExpression.HashParametersExpression(parameters);
                    }

                    if (!moveNext)
                    {
                        yield break;
                    }

                    item = enumerator.Current;
                }

                yield return item is Expression expression ? Visit(expression) : item;
            }
        }

        Dictionary<string, Expression> AccumulateParameters(IEnumerator<object> enumerator, out bool moveNext)
        {
            moveNext = true;
            var parameters = new Dictionary<string, Expression>();

            var item = enumerator.Current;
            while (item is HashParameterAssignmentExpression parameterAssignment)
            {
                item = GetNext(enumerator);
                if (item is Expression value)
                {
                    if (value is PathExpression path
                        && (path.Path == "true" || path.Path == "false"))
                    {
                        parameters.Add(parameterAssignment.Name,
                            Expression.Convert(Expression.Constant(path.Path == "true"), typeof(object)));
                    }
                    else
                    {
                        parameters.Add(parameterAssignment.Name, Visit(value));
                    }
                }
                else
                {
                    throw new HandlebarsCompilerException(string.Format("Unexpected token '{0}', expected an expression", item));
                }

                moveNext = enumerator.MoveNext();
                if (!moveNext)
                {
                    break;
                }
                item = enumerator.Current;
            }

            return parameters;
        }

        Expression Visit(Expression expression)
        {
            if (expression is HelperExpression helperExpression)
            {
                var originalArguments = helperExpression.Arguments.ToArray();
                var arguments = ConvertTokens(originalArguments)
                    .Cast<Expression>()
                    .ToArray();
                if (!arguments.SequenceEqual(originalArguments))
                {
                    return HandlebarsExpression.Helper(
                        helperExpression.HelperName,
                        arguments);
                }
            }
            if (expression is SubExpressionExpression subExpression)
            {
                Expression childExpression = Visit(subExpression.Expression);
                if (childExpression != subExpression.Expression)
                {
                    return HandlebarsExpression.SubExpression(childExpression);
                }
            }
            return expression;
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

