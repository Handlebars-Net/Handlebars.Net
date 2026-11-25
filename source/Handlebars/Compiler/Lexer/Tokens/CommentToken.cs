using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class CommentToken : Token
    {
        private readonly string _comment;

        public CommentToken(string comment, IReaderContext? context = null)
        {
            _comment = comment.Trim('-', ' ');
            Context = context;
        }

        public IReaderContext? Context { get; }

        public override TokenType Type
        {
            get { return TokenType.Comment; }
        }

        public override string Value
        {
            get { return _comment; }
        }
    }
}

