using System;

namespace Handlebars.Compiler.Lexer
{
    internal class PartialToken : Token
    {
        public PartialToken()
        {
        }

        public override TokenType Type
        {
            get { return TokenType.Partial; }
        }

        public override string Value
        {
            get { return ">"; }
        }
    }
}

