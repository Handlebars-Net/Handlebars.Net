namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class StartExpressionToken : ExpressionScopeToken
    {
        public StartExpressionToken(bool isEscaped, bool trimWhitespace, bool isRaw, IReaderContext context)
        {
            Context = context;
            IsEscaped = isEscaped;
            TrimPreceedingWhitespace = trimWhitespace;
            IsRaw = isRaw;
        }

        public IReaderContext Context { get; }
        
        public bool IsEscaped { get; }

        public bool TrimPreceedingWhitespace { get; }

        public bool IsRaw { get; }

        public override string Value => IsRaw ? "{{{{" : IsEscaped ? "{{" : "{{{";

        public override TokenType Type => TokenType.StartExpression;
    }
}

