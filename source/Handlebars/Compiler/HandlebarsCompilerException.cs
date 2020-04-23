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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        internal HandlebarsCompilerException(string message, IReaderContext context = null)
            : this(message, null, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public HandlebarsCompilerException(string message, Exception innerException)
            : base(message, innerException, null)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="context"></param>
        internal HandlebarsCompilerException(string message, Exception innerException, IReaderContext context = null)
            : base(message, innerException, context)
        {
        }
    }
}

