namespace HandlebarsDotNet.Compiler.Lexer
{
    internal abstract class Parser
    {
        public abstract Token Parse(ExtendedStringReader reader);
    }
}

