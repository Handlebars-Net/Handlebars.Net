namespace HandlebarsDotNet.Collections
{
    /// <summary>
    /// Represents collection of <typeparamref name="TValue"/> indexed by <typeparamref name="TKey"/>
    /// <para>Interface is similar to <see cref="IDictionary{TKey,TValue}"/> but exposes smaller set of APIs</para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IIndexed<TKey, TValue> : IReadOnlyIndexed<TKey, TValue>
    {
        void AddOrReplace(in TKey key, in TValue value);
        new TValue this[in TKey key] { get; set; }
        void Clear();
    }
}