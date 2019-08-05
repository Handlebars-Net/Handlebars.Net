using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class AssignmentToken : Token
    {
        /// <inheritdoc />
        public AssignmentToken() : base("=")
        {
        }

        public override TokenType Type
        {
            get { return TokenType.Assignment; }
        }

        public override string Value
        {
            get { return "="; }
        }
    }
}

