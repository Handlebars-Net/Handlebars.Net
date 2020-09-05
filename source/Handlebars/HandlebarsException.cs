using System;

namespace HandlebarsDotNet
{
    /// <summary>
    /// General <c>Handlebars</c> exception
    /// </summary>
    public class HandlebarsException : Exception
    {
        public HandlebarsException(string message)
            : this(message, null, null)
        {
        }
        
        internal HandlebarsException(string message, IReaderContext context = null)
            : this(message, null, context)
        {
        }
        
        public HandlebarsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
        internal HandlebarsException(string message, Exception innerException, IReaderContext context = null)
            : base(FormatMessage(message, context), innerException)
        {
        }
        
        private static string FormatMessage(string message, IReaderContext context)
        {
            if (context == null) return message;

            return $"{message}\n\nOccured at: {context.LineNumber}:{context.CharNumber}";
        }
    }
}

