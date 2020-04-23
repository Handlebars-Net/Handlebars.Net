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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        internal HandlebarsParserException(string message, IReaderContext context = null)
            : this(message, null, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public HandlebarsParserException(string message, Exception innerException)
            : base(message, innerException, null)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="context"></param>
        internal HandlebarsParserException(string message, Exception innerException, IReaderContext context = null)
            : base(message, innerException, context)
        {
        }
    }
}

