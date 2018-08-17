using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace HandlebarsDotNet.Compiler
{
    internal class ExpressionBuilder
    {
        private readonly HandlebarsConfiguration _configuration;

        public ExpressionBuilder(HandlebarsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Expression> ConvertTokensToExpressions(IEnumerable<object> tokens)
        {
            tokens = CommentAndLayoutConverter.Convert(tokens);
            tokens = LiteralConverter.Convert(tokens);
            tokens = HelperConverter.Convert(tokens, _configuration);
            tokens = PathConverter.Convert(tokens);
            // HashParameters converter needs to run before and after SubExpressionConverter
            tokens = HashParametersConverter.Convert(tokens);
            tokens = SubExpressionConverter.Convert(tokens);
            tokens = HashParametersConverter.Convert(tokens);
            tokens = PartialConverter.Convert(tokens);
            tokens = HelperArgumentAccumulator.Accumulate(tokens);
            tokens = ExpressionScopeConverter.Convert(tokens);
            tokens = WhitespaceRemover.Remove(tokens);
            tokens = StaticConverter.Convert(tokens);
            tokens = BlockAccumulator.Accumulate(tokens, _configuration);
            return tokens.Cast<Expression>();
        }
    }
}
