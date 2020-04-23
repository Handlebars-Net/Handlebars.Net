namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class BlockParamsParser : Parser
    {
        public override Token Parse(ExtendedStringReader reader)
        {
            var context = reader.GetContext();
            var buffer = AccumulateWord(reader);
            return !string.IsNullOrEmpty(buffer) 
                ? Token.BlockParams(buffer, context) 
                : null;
        }
        
        private static string AccumulateWord(ExtendedStringReader reader)
        {
            var buffer = StringBuilderPool.Shared.GetObject();

            try
            {
                if (reader.Peek() != '|') return null;
                
                reader.Read();
            
                while (reader.Peek() != '|')
                {
                    buffer.Append((char) reader.Read());
                }

                reader.Read();

                var accumulateWord = buffer.ToString().Trim();
                if(string.IsNullOrEmpty(accumulateWord)) throw new HandlebarsParserException($"BlockParams expression is not valid", reader.GetContext());
            
                return accumulateWord;
            }
            finally
            {
                StringBuilderPool.Shared.PutObject(buffer);
            }
        }
    }
}