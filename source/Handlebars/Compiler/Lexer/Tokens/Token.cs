using System;

namespace Handlebars.Compiler.Lexer
{
    internal abstract class Token
    {
        public abstract TokenType Type { get; }
        public abstract string Value { get; }

        public static StaticToken Static(string value)
        {
            return new StaticToken (value);
        }

        public static LiteralExpressionToken Literal(string value, string delimiter = null)
        {
            return new LiteralExpressionToken (value, delimiter);
        }

        public static WordExpressionToken Word(string word)
        {
            return new WordExpressionToken(word);
        }

		public static StartExpressionToken StartExpression(bool isEscaped)
        {
            return new StartExpressionToken(isEscaped);
        }

		public static EndExpressionToken EndExpression(bool isEscaped)
        {
            return new EndExpressionToken(isEscaped);
        }

        public static CommentToken Comment(string comment)
        {
            return new CommentToken(comment);
        }

        public static PartialToken Partial()
        {
            return new PartialToken();
        }
    }
}

