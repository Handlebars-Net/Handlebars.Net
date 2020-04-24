using System.Collections.Generic;

namespace HandlebarsDotNet.Collections
{
    internal interface IRefDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TValue : struct
    {
        new ref TValue this[TKey key] { get; }
    }
}