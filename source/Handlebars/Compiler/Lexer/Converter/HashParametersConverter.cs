using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class HashParametersConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new HashParametersConverter().ConvertTokens(sequence).ToList();
        }

        private HashParametersConverter() { }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;

                Dictionary<string, object> parameters = null;
                while (item is PathExpression param)
                {
                    item = GetNext(enumerator);
                    if (!(item is AssignmentToken))
                    {
                        yield return param;
                        continue;
                    }
                    if (!enumerator.MoveNext())
                    {
                        throw new HandlebarsException("No value assigned to parameter");
                    }
                    if (enumerator.Current is StartSubExpressionToken)
                    {
                        yield return param;
                        yield return item;
                        yield return enumerator.Current;
                        item = GetNext(enumerator);
                        continue;
                    }
                    if (parameters == null)
                    {
                        parameters = new Dictionary<string, object>();
                    }
                    if (enumerator.Current is PathExpression valuePath)
                    {
                        if (valuePath.Path == "true")
                        {
                            parameters.Add(param.Path, Expression.Constant(true));
                        }
                        else if (valuePath.Path == "false")
                        {
                            parameters.Add(param.Path, Expression.Constant(false));
                        }
                        else
                        {
                            parameters.Add(param.Path, valuePath);
                        }
                    }
                    else
                    {
                        parameters.Add(param.Path, enumerator.Current);
                    }
                    item = GetNext(enumerator);
                }
                if (parameters != null)
                {
                    yield return HandlebarsExpression.HashParametersExpression(parameters);
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

