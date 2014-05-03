using System;

namespace Handlebars.Compiler.Lexer
{
    internal class EndExpressionToken : ExpressionScopeToken
    {
        public override string Value
        {
            get { return "}}"; }
        }

        public override TokenType Type
        {
            get { return TokenType.EndExpression; }
        }
    }
}

