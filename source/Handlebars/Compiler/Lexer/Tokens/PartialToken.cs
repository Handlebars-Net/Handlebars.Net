namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class PartialToken : Token
    {
        public PartialToken(IReaderContext context = null)
        {
            Context = context;
        }

        public IReaderContext Context { get; }
        
        public override TokenType Type => TokenType.Partial;

        public override string Value => ">";
    }
}

