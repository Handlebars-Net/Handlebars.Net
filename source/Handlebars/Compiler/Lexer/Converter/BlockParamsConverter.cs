using System;
using System.Collections.Generic;
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
            var foundBlockParams = false;
            foreach (var item in sequence)
            {
                switch (item)
                {
                    case BlockParameterToken blockParameterToken:
                        BlockParamsFound(ref foundBlockParams, blockParameterToken);
                        ConvertBlockParam(blockParameterToken, result);
                        continue;
                    
                    case EndExpressionToken _:
                        foundBlockParams = false;
                        break;
                }

                result.Add(item);
            }

            return result;
        }

        private static void ConvertBlockParam(BlockParameterToken blockParameterToken, List<object> result)
        {
            var pathExpression = result[result.Count - 1] as PathExpression;
            VerifyBlockParamsSyntax(blockParameterToken, pathExpression);

            result[result.Count - 1] = HandlebarsExpression.BlockParams(pathExpression!.Path, blockParameterToken.Value);
        }

        private static void VerifyBlockParamsSyntax(BlockParameterToken blockParameterToken, PathExpression pathExpression)
        {
            if (pathExpression == null) throw new HandlebarsCompilerException("blockParams definition has incorrect syntax", blockParameterToken.Context);
            if (!string.Equals("as", pathExpression.Path, StringComparison.OrdinalIgnoreCase)) throw new HandlebarsCompilerException("blockParams definition has incorrect syntax", blockParameterToken.Context);
        }

        private static void BlockParamsFound(ref bool foundBlockParams, BlockParameterToken blockParameterToken)
        {
            if (foundBlockParams) throw new HandlebarsCompilerException("multiple blockParams expressions are not supported", blockParameterToken.Context);
            foundBlockParams = true;
        }
    }
}