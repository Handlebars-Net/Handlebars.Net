using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockParamsConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new BlockParamsConverter().ConvertTokens(sequence).ToList();
        }

        private BlockParamsConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var result = new List<object>();
            bool foundBlockParams = false;
            foreach (var item in sequence)
            {
                switch (item)
                {
                    case BlockParameterToken blockParameterToken when foundBlockParams:
                        throw new HandlebarsCompilerException("multiple blockParams expressions are not supported");
                    
                    case BlockParameterToken blockParameterToken:
                        foundBlockParams = true;
                        if (!(result.Last() is PathExpression pathExpression))
                            throw new HandlebarsCompilerException("blockParams definition has incorrect syntax");
                        if (!string.Equals("as", pathExpression.Path, StringComparison.OrdinalIgnoreCase))
                            throw new HandlebarsCompilerException("blockParams definition has incorrect syntax");

                        result[result.Count - 1] =
                            HandlebarsExpression.BlockParams(pathExpression.Path, blockParameterToken.Value);
                        break;
                    
                    case EndExpressionToken _:
                        foundBlockParams = false;
                        result.Add(item);
                        break;

                    default:
                        result.Add(item);
                        break;
                }
            }

            return result;
        }
    }
}