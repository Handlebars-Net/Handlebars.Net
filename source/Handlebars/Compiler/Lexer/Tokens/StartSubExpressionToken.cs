using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class StartSubExpressionToken : ExpressionScopeToken
    {
        public StartSubExpressionToken() : base("(")
        {
        }

        public override string Value
        {
            get { return "("; }
        }

        public override TokenType Type
        {
            get { return TokenType.StartSubExpression; }
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}

