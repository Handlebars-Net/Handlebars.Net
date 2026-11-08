namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class LayoutToken : Token
    {
        private readonly string _layout;

        public LayoutToken(string layout, IReaderContext? context = null)
        {
            _layout = layout.Trim('-', ' ');
            Context = context;
        }

        public IReaderContext? Context { get; }

        public override TokenType Type
        {
            get { return TokenType.Layout; }
        }

        public override string Value
        {
            get { return _layout; }
        }
    }
}