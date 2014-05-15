using System;
using System.IO;
using System.Text;

namespace Handlebars.Compiler.Lexer
{
    internal class CommentParser : Parser
    {
        public override Token Parse(TextReader reader)
        {
            CommentToken token = null;
            if (IsComment (reader))
            {
                var buffer = AccumulateComment (reader);
                token = Token.Comment (buffer);
            }
            return token;
        }

        private bool IsComment(TextReader reader)
        {
            var peek = (char)reader.Peek ();
            return peek == '!';
        }

        private string AccumulateComment(TextReader reader)
        {
            StringBuilder buffer = new StringBuilder ();
            while (true)
            {
                var peek = (char)reader.Peek ();
                if (peek == '}')
                {
                    break;
                }
                var node = reader.Read ();
                if(node == -1)
                {
                    throw new InvalidOperationException("Reached end of template in the middle of a comment");
                }
                else
                {
                    buffer.Append ((char)node);
                }
            }
            return buffer.ToString ();
        }
    }
}

