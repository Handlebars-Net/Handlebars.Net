using System;

namespace HandlebarsDotNet
{
    /// <summary>
    /// General <c>Handlebars</c> exception
    /// </summary>
    public class HandlebarsException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public HandlebarsException(string message)
            : this(message, null, null)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        internal HandlebarsException(string message, IReaderContext context = null)
            : this(message, null, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public HandlebarsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="context"></param>
        internal HandlebarsException(string message, Exception innerException, IReaderContext context = null)
            : base(FormatMessage(message, context), innerException)
        {
        }
        
        private static string FormatMessage(string message, IReaderContext context)
        {
            if (context == null) return message;

            return $"{message}\nOccured at:{context.LineNumber}:{context.CharNumber}";
        }
    }
}

