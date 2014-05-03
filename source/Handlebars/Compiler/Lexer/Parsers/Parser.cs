using System;
using System.IO;

namespace Handlebars.Compiler.Lexer
{
    internal abstract class Parser
    {
        public abstract Token Parse (TextReader reader);
    }
}

