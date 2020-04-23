using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    internal class HashParameterDictionary : Dictionary<string, object>
    {
        public HashParameterDictionary()
            :base(StringComparer.OrdinalIgnoreCase)
        {
            
        }
    }
}