using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HandlebarsDotNet.Compiler.Lexer
{
    internal class BlockParamsParser : Parser
    {
        public override Token Parse(TextReader reader)
        {
            var buffer = AccumulateWord(reader);
            return !string.IsNullOrEmpty(buffer) 
                ? Token.BlockParams(buffer) 
                : null;
        }
        
        private static string AccumulateWord(TextReader reader)
        {
            var buffer = new StringBuilder();
            
            if (reader.Peek() != '|') return null;
                
            reader.Read();
            
            while (reader.Peek() != '|')
            {
                buffer.Append((char) reader.Read());
            }

            reader.Read();

            var accumulateWord = buffer.ToString().Trim();
            if(string.IsNullOrEmpty(accumulateWord)) throw new HandlebarsParserException($"BlockParams expression is not valid");
            
            return accumulateWord;
        }
    }
}