namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class EndExpressionToken : ExpressionScopeToken
    {
        public EndExpressionToken(bool isEscaped, bool trimWhitespace, bool isRaw, IReaderContext context)
        {
            IsEscaped = isEscaped;
            TrimTrailingWhitespace = trimWhitespace;
            IsRaw = isRaw;
            Context = context;
        }

        public bool IsEscaped { get; }

        public bool TrimTrailingWhitespace { get; }

        public bool IsRaw { get; }
        public IReaderContext Context { get; }

        public override string Value => IsRaw ? "}}}}" : IsEscaped ? "}}" : "}}}";

        public override TokenType Type => TokenType.EndExpression;
    }
}

