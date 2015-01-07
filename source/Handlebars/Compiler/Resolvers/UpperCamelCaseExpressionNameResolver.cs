using System;

namespace Handlebars.Compiler.Resolvers
{
    public class UpperCamelCaseExpressionNameResolver : IExpressionNameResolver
    {
        public string ResolveExpressionName(string expressionName)
        {
            if (string.IsNullOrEmpty(expressionName))
                return expressionName;

            var containsUnderScores = expressionName.IndexOf("_", StringComparison.OrdinalIgnoreCase) >= 0;
            var containsDots = expressionName.IndexOf(".", StringComparison.OrdinalIgnoreCase) >= 0;

            if (char.IsUpper(expressionName[0]) && !containsUnderScores && !containsDots)
                return expressionName;

            var chars = expressionName.ToCharArray();

            if (containsUnderScores)
            {
                var index = 0;
                var buffer = new char[chars.Length];

                chars[0] = char.ToUpperInvariant(chars[0]);
                for (var i = 0; i < chars.Length; i++)
                {
                    var hasNext = (i + 1 < chars.Length);
                    var isUnderscore = chars[i] == '_';
                    if (hasNext && isUnderscore)
                    {
                        chars[i + 1] = char.ToUpperInvariant(chars[i + 1]);
                    }
                    else
                    {
                        buffer[index++] = chars[i];
                    }
                }

                return new string(buffer).TrimEnd('\0');
            }

            chars[0] = char.ToUpperInvariant(chars[0]);
            for (var i = 0; i < chars.Length; i++)
            {
                var hasNext = (i + 1 < chars.Length);
                var isDot = chars[i] == '.';
                if (isDot && hasNext)
                    chars[i + 1] = char.ToUpperInvariant(chars[i + 1]);
            }

            return new string(chars);
        }
    }
}