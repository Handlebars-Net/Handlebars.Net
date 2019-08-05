using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal abstract class ExpressionScopeToken : Token
    {
        /// <inheritdoc />
        protected ExpressionScopeToken(string originalValue) : base(originalValue)
        {
        }
    }
}

