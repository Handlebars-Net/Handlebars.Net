using System;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class StartExpressionToken : ExpressionScopeToken
    {
        private readonly bool _isEscaped;
        private readonly bool _trimWhitespace;
        private readonly bool _isRaw;

        public StartExpressionToken(bool isEscaped, bool trimWhitespace, bool isRaw)
        {
            _isEscaped = isEscaped;
            _trimWhitespace = trimWhitespace;
            _isRaw = isRaw;
        }

        public bool IsEscaped
        {
            get { return _isEscaped; }
        }

        public bool TrimPreceedingWhitespace
        {
            get { return _trimWhitespace; }
        }

        public bool IsRaw
        {
            get { return _isRaw; }
        }

        public override string Value
        {
            get { return IsRaw ? "{{{{" : IsEscaped ? "{{" : "{{{"; }
        }

        public override TokenType Type
        {
            get { return TokenType.StartExpression; }
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}

