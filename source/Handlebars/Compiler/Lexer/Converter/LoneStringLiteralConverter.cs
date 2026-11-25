using System.Collections.Generic;
using System.Linq;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler
{
    /// <summary>
    /// Handlebars.js resolves a mustache consisting of a single quoted string, e.g. <c>{{'foo bar'}}</c>,
    /// as a path made of that one literal segment, i.e. the same as <c>{{[foo bar]}}</c>.
    /// This rewrites such literals into exactly what <see cref="HelperConverter"/> produces for the equivalent
    /// segment literal, so they go through normal helper / path resolution.
    /// Literals that cannot be expressed as a segment literal (empty, or containing <c>]</c>) are left untouched.
    /// </summary>
    /// <remarks>
    /// Must run after <see cref="RawHelperAccumulator"/>, which rebuilds raw-block content from the original tokens.
    /// </remarks>
    internal static class LoneStringLiteralConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence, ICompiledHandlebarsConfiguration configuration)
        {
            var tokens = sequence.ToList();
            for (var index = 1; index < tokens.Count - 1; index++)
            {
                if (tokens[index] is LiteralExpressionToken { IsDelimitedLiteral: true } literal
                    && tokens[index - 1] is StartExpressionToken start
                    && tokens[index + 1] is EndExpressionToken
                    && literal.Value.Length > 0
                    && literal.Value.IndexOf(']') < 0)
                {
                    var name = $"[{literal.Value}]";
                    tokens[index] = HelperConverter.IsRegisteredHelperName(name, configuration)
                        ? HandlebarsExpression.Helper(name, false, start.IsRaw, literal.Context)
                        : Token.Word(name, literal.Context);
                }
            }

            return tokens;
        }
    }
}
