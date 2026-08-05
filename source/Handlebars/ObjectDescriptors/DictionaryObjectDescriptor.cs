using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.MemberAccessors;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.ObjectDescriptors
{
    public sealed class DictionaryObjectDescriptor : IObjectDescriptorProvider
    {
        private static readonly Type Type = typeof(IDictionary);

        private static readonly MethodInfo Factory = new Func<ObjectDescriptor>(ObjectDescriptorFactory<IDictionary>)
            .GetMethodInfo().GetGenericMethodDefinition();
        
        private static readonly DictionaryMemberAccessor DictionaryMemberAccessor = new DictionaryMemberAccessor();
        
        private static readonly Func<ObjectDescriptor, object, IEnumerable> GetProperties = (descriptor, arg) => ((IDictionary) arg).Keys;

        public bool TryGetDescriptor(Type type, [MaybeNullWhen(false)] out ObjectDescriptor value)
        {
            if (!Type.IsAssignableFrom(type))
            {
                value = ObjectDescriptor.Empty;
                return false;
            }
            
            value = (ObjectDescriptor) Factory.MakeGenericMethod(type).Invoke(null, ArrayEx.Empty<object>())!;
            return true;
        }

        private static ObjectDescriptor ObjectDescriptorFactory<T>()
            where T : class, IDictionary
        {
            return new ObjectDescriptor(typeof(T), DictionaryMemberAccessor, GetProperties, self => new DictionaryIterator<T>());
        }
    }
}