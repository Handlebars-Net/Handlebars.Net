using System;
using System.Collections.Generic;
using System.Reflection;

namespace HandlebarsDotNet.IO
{
    public class ReadOnlyCollectionFormatterProvider : IFormatterProvider
    {
        private static readonly Type CollectionFormatterType = typeof(ReadOnlyCollectionFormatter<,>);
        private static readonly Type CollectionType = typeof(IReadOnlyCollection<>);

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
        
        private class ReadOnlyCollectionFormatter<TValue, TCollection> : IFormatter
            where TCollection: class, IReadOnlyCollection<TValue>
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