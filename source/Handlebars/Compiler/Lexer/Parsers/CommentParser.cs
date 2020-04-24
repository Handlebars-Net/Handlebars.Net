using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class CommentParser : Parser
    {
        public override Token Parse(ExtendedStringReader reader)
        {
            if (!IsComment(reader)) return null;
         
            Token token = null;
            var buffer = AccumulateComment(reader).Trim();
            if (buffer.StartsWith("<")) //syntax for layout is {{<! layoutname }} - i.e. its inside a comment block
            {
                token = Token.Layout(buffer.Substring(1).Trim());
            }

            token = token ?? Token.Comment(buffer);
            return token;
        }

        private static bool IsComment(ExtendedStringReader reader)
        {
            var peek = (char)reader.Peek();
            return peek == '!';
        }

        private static string AccumulateComment(ExtendedStringReader reader)
        {
            reader.Read();
            bool? escaped = null;
            using(var container = StringBuilderPool.Shared.Use())
            {
                var buffer = container.Value;
                while (true)
                {
                    if (escaped == null)
                    {
                        escaped = CheckIfEscaped(reader, buffer);
                    }
                    if (IsClosed(reader, buffer, escaped.Value))
                    {
                        break;
                    }
                    var node = reader.Read();
                    if (node == -1)
                    {
                        throw new HandlebarsParserException("Reached end of template in the middle of a comment", reader.GetContext());
                    }
                    else
                    {
                        buffer.Append((char)node);
                    }
                }
                
                return buffer.ToString();
            }
        }

        private static bool IsClosed(ExtendedStringReader reader, StringBuilder buffer, bool isEscaped)
        {
            return isEscaped && CheckIfEscaped(reader, buffer) && CheckIfStatementClosed(reader) || !isEscaped && CheckIfStatementClosed(reader);
        }

        private static bool CheckIfStatementClosed(ExtendedStringReader reader)
        {
            return (char) reader.Peek() == '}';
        }

        private static bool CheckIfEscaped(ExtendedStringReader reader, StringBuilder buffer)
        {
            if ((char) reader.Peek() != '-') return false;
         
            var escaped = false;
            var first = reader.Read();
            if ((char)reader.Peek() == '-')
            {
                reader.Read();
                escaped = true;
            }
            else
            {
                buffer.Append(first);
            }

            return escaped;
        }
    }
}

