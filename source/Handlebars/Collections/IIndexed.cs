namespace HandlebarsDotNet.Collections
{
    public interface IIndexed<TKey, TValue>
    {
        bool TryGetValue(in TKey key, out TValue value);
    }
}