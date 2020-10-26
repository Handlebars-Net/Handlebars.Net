using System;

namespace HandlebarsDotNet.Compiler
{
    public class HandlebarsUndefinedBindingException : Exception
    {
        public HandlebarsUndefinedBindingException(string path, string missingKey) : base(missingKey + " is undefined")
        {
            Path = path;
            MissingKey = missingKey;
        }
        
        public HandlebarsUndefinedBindingException(string path, UndefinedBindingResult undefined) : base(undefined.Value + " is undefined")
        {
            Path = path;
            MissingKey = undefined.Value;
        }

        public string Path { get; }

        public string MissingKey { get; }
    }
}
