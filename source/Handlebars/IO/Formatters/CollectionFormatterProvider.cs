using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.IO
{
    public class CollectionFormatterProvider : IFormatterProvider
    {
        private static readonly Type CollectionFormatterType = typeof(CollectionFormatter<,>);
        private static readonly Type CollectionType = typeof(ICollection<>);

        public bool TryCreateFormatter(Type type, out IFormatter formatter)
        {
            if (!type.GetTypeInfo().IsClass || !type.IsAssignableToGenericType(CollectionType, out var genericType))
            {
                formatter = null;
                return false;
            }

            var genericTypeArgument = genericType.GetGenericArguments()[0];
            var targetType = CollectionFormatterType.MakeGenericType(genericTypeArgument, type);
            formatter = (IFormatter) Activator.CreateInstance(targetType);
            return true;
        }
        
        private class CollectionFormatter<TValue, TCollection> : IFormatter
            where TCollection: class, ICollection<TValue>
        {
            public void Format<T>(T value, in EncodedTextWriter writer)
            {
                var index = 0;
                var collection = value as TCollection;
                var lastIndex = collection!.Count - 1;
                using var enumerator = collection.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    writer.Write(enumerator.Current);
                    if (index != lastIndex)
                    {
                        writer.Write(",", false);
                    }

                    ++index;
                }   
            }
        }
    }
}