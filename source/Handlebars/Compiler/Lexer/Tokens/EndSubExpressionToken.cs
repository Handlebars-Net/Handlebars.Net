namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class EndSubExpressionToken : ExpressionScopeToken
    {
        public IReaderContext Context { get; }

        public EndSubExpressionToken(IReaderContext context)
        {
            Context = context;
        }

        public override string Value { get; } =  ")";

        public override TokenType Type => TokenType.EndSubExpression;
    }
}

