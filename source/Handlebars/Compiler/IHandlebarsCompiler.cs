using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using HandlebarsDotNet.Compiler.Lexer;

namespace HandlebarsDotNet.Compiler {
	public interface IHandlebarsCompiler {
		IEnumerable<IToken> Tokenize( TextReader source );
		IEnumerable<Expression> ExpressionBuilder( IEnumerable<IToken> tokens );
		Action<TextWriter, object> FunctionBuilder( IEnumerable<Expression> expressions, string templatePath = null );
		Action<TextWriter, object> Compile( TextReader source );
		Action<TextWriter, object> CompileView( string templatePath );

	}
}
