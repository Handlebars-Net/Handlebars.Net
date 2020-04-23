namespace HandlebarsDotNet.Compiler.Lexer
{
    internal abstract class Token
    {
        public abstract TokenType Type { get; }

        public abstract string Value { get; }

        public sealed override string ToString()
        {
            return Value;
        }

        public static StaticToken Static(string value, IReaderContext context = null)
        {
            return new StaticToken(value, context);
        }

        public static LiteralExpressionToken Literal(string value, string delimiter = null, IReaderContext context = null)
        {
            return new LiteralExpressionToken(value, delimiter, context);
        }

        public static WordExpressionToken Word(string word, IReaderContext context = null)
        {
            return new WordExpressionToken(word, context);
        }

        public static StartExpressionToken StartExpression(bool isEscaped, bool trimWhitespace, bool isRaw, IReaderContext context = null)
        {
            return new StartExpressionToken(isEscaped, trimWhitespace, isRaw, context);
        }

        public static EndExpressionToken EndExpression(bool isEscaped, bool trimWhitespace, bool isRaw, IReaderContext context = null)
        {
            return new EndExpressionToken(isEscaped, trimWhitespace, isRaw, context);
        }

        public static CommentToken Comment(string comment)
        {
            return new CommentToken(comment);
        }

        public static PartialToken Partial(IReaderContext context = null)
        {
            return new PartialToken(context);
        }

        public static LayoutToken Layout(string layout)
        {
            return new LayoutToken(layout);
        }

        public static StartSubExpressionToken StartSubExpression()
        {
            return new StartSubExpressionToken();
        }

        public static EndSubExpressionToken EndSubExpression(IReaderContext context)
        {
            return new EndSubExpressionToken(context);
        }

        public static AssignmentToken Assignment(IReaderContext context)
        {
            return new AssignmentToken(context);
        }
        
        public static BlockParameterToken BlockParams(string blockParams, IReaderContext context)
        {
            return new BlockParameterToken(blockParams, context);
        }
    }
}
