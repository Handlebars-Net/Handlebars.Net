using System;

namespace Handlebars.Compiler.Lexer
{
    internal class EndExpressionToken : ExpressionScopeToken
    {
		private readonly bool _isEscaped;

		public EndExpressionToken(bool isEscaped)
		{
			_isEscaped = isEscaped;
		}

		public bool IsEscaped
		{
			get { return _isEscaped; }
		}

        public override string Value
        {
			get { return IsEscaped ? "}}" : "}}}"; }
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

