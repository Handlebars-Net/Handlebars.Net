using System;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Represents errors occured in Handlebar's runtime
    /// </summary>
    public class HandlebarsRuntimeException : HandlebarsException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public HandlebarsRuntimeException(string message)
            : this(message, null, null)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        internal HandlebarsRuntimeException(string message, IReaderContext context = null)
            : this(message, null, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public HandlebarsRuntimeException(string message, Exception innerException)
            : base(message, innerException, null)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="context"></param>
        internal HandlebarsRuntimeException(string message, Exception innerException, IReaderContext context = null)
            : base(message, innerException, context)
        {
        }
    }
}
