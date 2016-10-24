

namespace HandlebarsDotNet.Compiler.Lexer
{
	public interface IToken {
		TokenType Type { get; }
		string Value { get; }
	}
}
