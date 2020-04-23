using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal abstract class TokenConverter
    {
        public abstract IEnumerable<object> ConvertTokens(IEnumerable<object> sequence);
    }
}

