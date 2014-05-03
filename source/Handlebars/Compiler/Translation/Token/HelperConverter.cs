using System;
using System.Collections.Generic;
using System.Linq;
using Handlebars.Compiler.Lexer;

namespace Handlebars.Compiler
{
    internal class HelperConverter : TokenConverter
    {
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
            while(enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if(item is StartExpressionToken)
                {
                    yield return item;
                    item = GetNext(enumerator);
                    if((item is WordExpressionToken) == false)
                    {
                        throw new HandlebarsCompilerException("Encountered a non-word symbol at the beginning of the handlebars expression.");
                    }
                    var word = item as WordExpressionToken;
                    if(IsRegisteredHelperName(word.Value))
                    {
                        yield return HandlebarsExpression.Helper(word.Value);
                    }
                    else
                    {
                        yield return word;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }


        private bool IsRegisteredHelperName(string name)
        {
            return _configuration.Helpers.ContainsKey(name) || _configuration.BlockHelpers.ContainsKey(name);
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}

