using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal abstract class ExpressionToken : Token
    {
        /// <inheritdoc />
        protected ExpressionToken(string originalValue) : base(originalValue)
        {
        }
    }
}

