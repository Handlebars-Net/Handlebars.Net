using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class EndExpressionToken : ExpressionScopeToken
    {
        private readonly bool _isEscaped;
        private readonly bool _trimWhitespace;
        private readonly bool _isRaw;

        public EndExpressionToken(bool isEscaped, bool trimWhitespace, bool isRaw)
        {
            _isEscaped = isEscaped;
            _trimWhitespace = trimWhitespace;
            _isRaw = isRaw;
        }

        public bool IsEscaped
        {
            get { return _isEscaped; }
        }

        public bool TrimTrailingWhitespace
        {
            get { return _trimWhitespace; }
        }

        public bool IsRaw
        {
            get { return _isRaw; }
        }

        public override string Value
        {
            get { return IsRaw ? "}}}}" : IsEscaped ? "}}" : "}}}"; }
        }

        public override TokenType Type
        {
            get { return TokenType.EndExpression; }
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}

