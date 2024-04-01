using HandlebarsDotNet.Pools;

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
            using(var container = StringBuilderPool.Shared.Use())
            {
                var buffer = container.Value;
                
                if (reader.Peek() != '|') return null;
                
                reader.Read();
            
                while (reader.Peek() != '|' && reader.Peek() != -1)
                {
                    buffer.Append((char) reader.Read());
                }

                reader.Read();

                var accumulateWord = buffer.ToString().Trim();
                if(string.IsNullOrEmpty(accumulateWord)) throw new HandlebarsParserException($"BlockParams expression is not valid", reader.GetContext());
            
                return accumulateWord;
            }
        }
    }
}