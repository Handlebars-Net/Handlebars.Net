using System;
using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class BlockParamsConverter : TokenConverter
    {
        private static readonly BlockParamsConverter Converter = new BlockParamsConverter();

        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return Converter.ConvertTokens(sequence);
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
                if (item is BlockParameterToken blockParameterToken)
                {
                    if(foundBlockParams) throw new HandlebarsCompilerException("multiple blockParams expressions are not supported", blockParameterToken.Context);
                    
                    foundBlockParams = true;
                    if(!(result[result.Count - 1] is PathExpression pathExpression)) throw new HandlebarsCompilerException("blockParams definition has incorrect syntax", blockParameterToken.Context);
                    if(!string.Equals("as", pathExpression.Path, StringComparison.OrdinalIgnoreCase)) throw new HandlebarsCompilerException("blockParams definition has incorrect syntax", blockParameterToken.Context);
                    
                    result[result.Count - 1] = HandlebarsExpression.BlockParams(pathExpression.Path, blockParameterToken.Value);
                }
                else
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}