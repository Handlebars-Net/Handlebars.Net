using System.Collections.Generic;

namespace HandlebarsDotNet.Collections
{
    public interface IAppendOnlyList<T> : IReadOnlyList<T>
    {
        void Add(T value);
    }
}