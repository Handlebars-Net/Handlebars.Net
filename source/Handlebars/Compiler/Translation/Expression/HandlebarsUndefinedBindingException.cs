using System;

namespace HandlebarsDotNet.Compiler
{
    /// <inheritdoc />
    public class HandlebarsUndefinedBindingException : Exception
    {
        
        /// <inheritdoc />
        public HandlebarsUndefinedBindingException(string path, string missingKey) : base(missingKey + " is undefined")
        {
            this.Path = path;
            this.MissingKey = missingKey;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MissingKey { get; set; }
    }
}
