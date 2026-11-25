using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    internal class CommentAndLayoutConverter : TokenConverter
    {
        private static readonly CommentAndLayoutConverter Converter = new CommentAndLayoutConverter();

        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return Converter.ConvertTokens(sequence).ToList();
        }

        private CommentAndLayoutConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            object? previous = null;
            foreach (var item in sequence)
            {
                yield return Convert(item, previous);
                previous = item;
            }
        }

        private static object Convert(object item, object? previous)
        {
            if (item is CommentToken commentToken)
            {
                EnsureStartsExpression(previous, commentToken.Value, commentToken.Context);
                return HandlebarsExpression.Comment(commentToken.Value);
            }

            if (item is LayoutToken layoutToken)
            {
                EnsureStartsExpression(previous, "<" + layoutToken.Value, layoutToken.Context);
                return HandlebarsExpression.Comment(null);
            }

            return item;
        }

        /// <summary>
        /// A '!' only introduces a comment (or layout) when it immediately follows the opening
        /// braces, e.g. "{{! comment }}". Anywhere else (such as "{{#if !value}}") it would end up
        /// as an argument that can never be compiled, so report it here with its location instead.
        /// </summary>
        private static void EnsureStartsExpression(object? previous, string value, IReaderContext? context)
        {
            if (previous is StartExpressionToken) return;

            throw new HandlebarsCompilerException(
                $"Unexpected '!{value}' inside an expression: '!' can only start a comment (e.g. '{{{{! comment }}}}') and is not a negation operator. Use '#unless' to negate a condition",
                context);
        }
    }
}
