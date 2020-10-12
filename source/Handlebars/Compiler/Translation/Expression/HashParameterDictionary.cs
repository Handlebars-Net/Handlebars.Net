using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Compiler
{
    public class HashParameterDictionary : Dictionary<string, object>
    {
        public HashParameterDictionary()
            :base(StringComparer.OrdinalIgnoreCase)
        {
            
        }
    }
}