using System;

namespace HandlebarsDotNet.IO
{
    internal class DefaultDateTimeFormatter : IFormatter, IFormatterProvider
    {
        private static readonly Type DateTimeType = typeof(DateTime);

        public void Format<T>(T value, in EncodedTextWriter writer)
        {
            if(!(value is DateTime dateTime)) throw new ArgumentException(" supposed to be DateTime", nameof(value));
            writer.Write(dateTime.ToString("O"), false);
        }

        public bool TryCreateFormatter(Type type, out IFormatter formatter)
        {
            if (type != DateTimeType)
            {
                formatter = null;
                return false;
            }

            formatter = this;
            return true;
        }
    }
}