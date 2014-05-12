using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Handlebars.Compiler.Lexer;

namespace Handlebars.Compiler
{
    internal abstract class TokenConverter
    {
        public abstract IEnumerable<object> ConvertTokens(IEnumerable<object> sequence);
    }
}

