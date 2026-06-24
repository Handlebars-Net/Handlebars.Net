using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.IO
{
    public class CollectionFormatterProvider : IFormatterProvider
    {
        private static readonly Type CollectionFormatterType = typeof(CollectionFormatter<,>);
        private static readonly Type CollectionType = typeof(ICollection<>);

        public bool TryCreateFormatter(Type type, [MaybeNullWhen(false)] out IFormatter formatter)
        {
            if (!type.GetTypeInfo().IsClass || !type.IsAssignableToGenericType(CollectionType, out var genericType))
            {
                formatter = null;
                return false;
            }

            var genericTypeArgument = genericType.GetGenericArguments()[0];
            var targetType = CollectionFormatterType.MakeGenericType(genericTypeArgument, type);
            formatter = (IFormatter) Activator.CreateInstance(targetType)!;
            return true;
        }
        
        private class CollectionFormatter<TValue, TCollection> : IFormatter
            where TCollection: class, ICollection<TValue>
        {
            public void Format<T>([NotNull] T value, in EncodedTextWriter writer)
            {
                if (value is not TCollection collection)
                {
                    Throw.ValueOfWrongTypeOrNull(nameof(value));
                    return;
                }
                var index = 0;
                var lastIndex = collection.Count - 1;
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

            private static class Throw
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                [DoesNotReturn]
                public static void ValueOfWrongTypeOrNull(string? paramName) => throw new ArgumentNullException(paramName);
            }
        }
    }
}