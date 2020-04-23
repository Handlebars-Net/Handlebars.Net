namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class AssignmentToken : Token
    {
        public AssignmentToken(IReaderContext context)
        {
            Context = context;
        }
        
        public IReaderContext Context { get; }

        public override TokenType Type => TokenType.Assignment;

        public override string Value => "=";
    }
}

