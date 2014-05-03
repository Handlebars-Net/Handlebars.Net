using System;

namespace Handlebars.Compiler.Lexer
{
    internal enum TokenType
	{
        Static = 0,
        StartExpression = 1,
        EndExpression = 2,
        Word = 3,
        Literal = 4,
        Structure = 5
	}
}

