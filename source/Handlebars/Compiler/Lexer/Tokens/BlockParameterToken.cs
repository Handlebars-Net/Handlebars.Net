using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class BlockParameterToken : Token
    {
        public BlockParameterToken(string value)
        {
            Value = value;
        }
        
        public override TokenType Type
        {
            get { return TokenType.BlockParams; }
        }

        public override string Value { get; }
    }
}