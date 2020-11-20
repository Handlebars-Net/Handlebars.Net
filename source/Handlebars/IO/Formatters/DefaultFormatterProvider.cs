using System;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.EqualityComparers;
using HandlebarsDotNet.IO.Formatters.DefaultFormatters;

namespace HandlebarsDotNet.IO
{
    public class DefaultFormatterProvider : IFormatterProvider
    {
        private static readonly DictionarySlim<Type, IFormatter, ReferenceEqualityComparer<Type>> Formatters = 
            new DictionarySlim<Type, IFormatter, ReferenceEqualityComparer<Type>>(12, new ReferenceEqualityComparer<Type>())
        {
            [typeof(DateTime)] = new DefaultDateTimeFormatter(),
            [typeof(int)] = new DefaultIntFormatter(),
            [typeof(bool)] = new DefaultBoolFormatter(),
            [typeof(char)] = new DefaultCharFormatter(),
            [typeof(float)] = new DefaultFloatFormatter(),
            [typeof(double)] = new DefaultDoubleFormatter(),
            [typeof(long)] = new DefaultLongFormatter(),
            [typeof(short)] = new DefaultShortFormatter(),
            [typeof(uint)] = new DefaultUIntFormatter(),
            [typeof(ulong)] = new DefaultULongFormatter(),
            [typeof(ushort)] = new DefaultUShortFormatter(),
            [typeof(decimal)] = new DefaultDecimalFormatter()
        };

        private static readonly DefaultObjectFormatter DefaultObjectFormatter = new DefaultObjectFormatter();
        
        public bool TryCreateFormatter(Type type, out IFormatter formatter)
        {
            if (!Formatters.TryGetValue(type, out formatter))
            {
                formatter = DefaultObjectFormatter;
            }

            return true;
        }
    }
}