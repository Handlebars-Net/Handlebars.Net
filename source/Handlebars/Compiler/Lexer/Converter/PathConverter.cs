using System;
using System.Collections.Generic;
using Handlebars.Compiler.Lexer;
using System.Linq.Expressions;
using System.Linq;

namespace Handlebars.Compiler
{
    internal class PathConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new PathConverter().ConvertTokens(sequence).ToList();
        }

        private PathConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            foreach(var item in sequence)
            {
                if(item is WordExpressionToken)
                {
                    yield return HandlebarsExpression.Path(((WordExpressionToken)item).Value);
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}

