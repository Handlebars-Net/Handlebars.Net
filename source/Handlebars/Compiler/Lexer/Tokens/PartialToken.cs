using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class PartialToken : Token
    {
        private readonly string _partialName;

        public PartialToken(string partialName)
        {
            _partialName = partialName;
        }

        public override TokenType Type
        {
            get { return TokenType.Partial; }
        }

        public override string Value
        {
            get { return _partialName; }
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}

