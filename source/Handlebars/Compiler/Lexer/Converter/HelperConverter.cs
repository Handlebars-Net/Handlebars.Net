using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class HelperConverter : TokenConverter
    {
        private static readonly HashSet<string> BuiltInHelpers = new HashSet<string>
        {
            "else", "each"
        };

        public static IEnumerable<object> Convert(
            IEnumerable<object> sequence,
            HandlebarsConfiguration configuration)
        {
            return new HelperConverter(configuration).ConvertTokens(sequence).ToList();
        }

        private readonly HandlebarsConfiguration _configuration;

        private HelperConverter(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (!(item is StartExpressionToken token))
                {
                    yield return item;
                    continue;
                }

                var isRaw = token.IsRaw;
                yield return token;
                item = GetNext(enumerator);
                switch (item)
                {
                    case Expression _:
                        yield return item;
                        continue;
                    case WordExpressionToken word when IsRegisteredHelperName(word.Value):
                        yield return HandlebarsExpression.Helper(word.Value);
                        break;
                    case WordExpressionToken word when IsRegisteredBlockHelperName(word.Value, isRaw):
                        yield return HandlebarsExpression.Helper(word.Value, isRaw);
                        break;
                    default:
                        yield return item;
                        break;
                }
            }
        }

        private bool IsRegisteredHelperName(string name)
        {
            return _configuration.Helpers.ContainsKey(name) || BuiltInHelpers.Contains(name);
        }

        private bool IsRegisteredBlockHelperName(string name, bool isRaw)
        {
            if (!isRaw && name[0] != '#') return false;
            name = name.Replace("#", "");
            return _configuration.BlockHelpers.ContainsKey(name) || BuiltInHelpers.Contains(name);
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

