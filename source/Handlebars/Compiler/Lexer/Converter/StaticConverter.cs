using System;
using System.Collections.Generic;
using HandlebarsDotNet.Compiler.Lexer;
using System.Linq;

namespace HandlebarsDotNet.Compiler
{
    internal class StaticConverter : TokenConverter
    {
        private static readonly StaticConverter Converter = new StaticConverter();

        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return Converter.ConvertTokens(sequence).ToList();
        }

        private StaticConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            foreach (var item in sequence)
            {
                if (!(item is StaticToken staticToken))
                {
                    yield return item;
                    continue;
                }

                if (staticToken.Value != string.Empty)
                {
                    yield return HandlebarsExpression.Static(staticToken.Value);
                }
            }
        }
    }
}

