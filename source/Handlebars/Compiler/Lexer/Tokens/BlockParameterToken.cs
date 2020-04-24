namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class BlockParameterToken : Token
    {
        public BlockParameterToken(string value, IReaderContext context = null)
        {
            Value = value;
            Context = context;
        }
        
        public override TokenType Type => TokenType.BlockParams;

        public override string Value { get; }
        public IReaderContext Context { get; }
    }
}