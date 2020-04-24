namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class StartSubExpressionToken : ExpressionScopeToken
    {
        public override string Value { get; } = "(";

        public override TokenType Type => TokenType.StartSubExpression;
    }
}

