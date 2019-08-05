namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class PartialToken : Token
    {
        /// <inheritdoc />
        public PartialToken(string originalValue) : base(originalValue)
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

        public override string ToString()
        {
            return Value;
        }
    }
}

