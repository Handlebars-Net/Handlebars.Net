using System;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Represents exceptions occured at compile time
    /// </summary>
    public class HandlebarsCompilerException : HandlebarsException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public HandlebarsCompilerException(string message)
            : this(message, null, null)
        {
        }
        
        internal HandlebarsCompilerException(string message, IReaderContext context = null)
            : this(message, null, context)
        {
        }
        
        public HandlebarsCompilerException(string message, Exception innerException)
            : base(message, innerException, null)
        {
        }
        
        internal HandlebarsCompilerException(string message, Exception innerException, IReaderContext context = null)
            : base(message, innerException, context)
        {
        }
    }
}

