using System;
using System.Diagnostics;

namespace Handlebars.Compiler
{
    [DebuggerDisplay("undefined")]
    internal class UndefinedBindingResult
    {
        public UndefinedBindingResult()
        {
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}

