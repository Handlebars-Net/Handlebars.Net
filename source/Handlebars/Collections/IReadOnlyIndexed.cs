using System.Collections.Generic;

namespace HandlebarsDotNet.Collections
{
    /// <summary>
    /// Represents read-only collection of <typeparamref name="TValue"/> indexed by <typeparamref name="TKey"/>
    /// <para>Interface is similar to <see cref="IReadOnlyDictionary{TKey,TValue}"/> but exposes smaller set of APIs</para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IReadOnlyIndexed<TKey, TValue> 
        : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        bool ContainsKey(in TKey key);
        bool TryGetValue(in TKey key, out TValue value);
        TValue this[in TKey key] { get; }
    }
}