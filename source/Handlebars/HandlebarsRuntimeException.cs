using System;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Represents errors occured in Handlebar's runtime
    /// </summary>
    public class HandlebarsRuntimeException : HandlebarsException
    {
        public HandlebarsRuntimeException(string message)
            : this(message, null, null)
        {
        }
        
        internal HandlebarsRuntimeException(string message, IReaderContext context = null)
            : this(message, null, context)
        {
        }
        
        public HandlebarsRuntimeException(string message, Exception innerException)
            : base(message, innerException, null)
        {
        }
        
        internal HandlebarsRuntimeException(string message, Exception innerException, IReaderContext context = null)
            : base(message, innerException, context)
        {
        }
    }
}
