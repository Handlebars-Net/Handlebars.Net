using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class AssignmentToken : Token
    {
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

