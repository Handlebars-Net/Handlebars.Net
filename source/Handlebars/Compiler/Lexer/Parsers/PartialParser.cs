using System;
using System.IO;

namespace Handlebars.Compiler.Lexer
{
    internal class PartialParser : Parser
    {            
        public override Token Parse(TextReader reader)
        {
            PartialToken token = null;
            if((char)reader.Peek() == '>')
            {
                token = Token.Partial();
            }
            return token;
        }
    }
}

