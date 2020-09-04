using System;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Represents exceptions occured at template parsing stage
    /// </summary>
    public class HandlebarsParserException : HandlebarsException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public HandlebarsParserException(string message)
            : this(message, null, null)
        {
        }
        
        internal HandlebarsParserException(string message, IReaderContext context = null)
            : this(message, null, context)
        {
        }
        
        public HandlebarsParserException(string message, Exception innerException)
            : base(message, innerException, null)
        {
        }
        
        internal HandlebarsParserException(string message, Exception innerException, IReaderContext context = null)
            : base(message, innerException, context)
        {
        }
    }
}

