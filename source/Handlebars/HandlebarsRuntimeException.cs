using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handlebars
{
    public class HandlebarsRuntimeException : HandlebarsException
    {
        public HandlebarsRuntimeException(string message)
            : base(message)
        {
        }

        public HandlebarsRuntimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
