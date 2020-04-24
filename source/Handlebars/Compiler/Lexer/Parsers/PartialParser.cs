namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class PartialParser : Parser
    {
        public override Token Parse(ExtendedStringReader reader)
        {
            PartialToken token = null;
            if ((char)reader.Peek() == '>')
            {
                token = Token.Partial(reader.GetContext());
            }
            return token;
        }
    }
}

