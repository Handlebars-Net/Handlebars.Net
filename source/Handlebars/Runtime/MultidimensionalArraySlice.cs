using System;
using System.Collections;
using System.Collections.Generic;

namespace HandlebarsDotNet.Runtime
{
    /// <summary>
    /// Represents a lazy view over one dimension of a true multi-dimensional <see cref="Array"/>
    /// (rank > 1, e.g. <c>int[,]</c>), fixing the leading indices supplied so far.
    /// Indexing or iterating a slice either yields the element (once every dimension has been
    /// fixed) or a narrower <see cref="MultidimensionalArraySlice"/> for the remaining dimensions.
    /// This lets a rank-N array be walked one dimension at a time, e.g. <c>{{grid.[0].[1]}}</c>
    /// or nested <c>{{#each}}</c> blocks, without ever needing to cast it to <c>T[]</c>.
    /// </summary>
    public sealed class MultidimensionalArraySlice : IReadOnlyList<object?>
    {
        private readonly Array _array;
        private readonly int[] _indices;

        internal MultidimensionalArraySlice(Array array, int[] indices)
        {
            _array = array;
            _indices = indices;
        }

        public int Count => _array.GetLength(_indices.Length);

        public object? this[int index]
        {
            get
            {
                var indices = new int[_indices.Length + 1];
                Array.Copy(_indices, indices, _indices.Length);
                indices[_indices.Length] = index;

                return indices.Length == _array.Rank
                    ? _array.GetValue(indices)
                    : new MultidimensionalArraySlice(_array, indices);
            }
        }

        public IEnumerator<object?> GetEnumerator()
        {
            var count = Count;
            for (var index = 0; index < count; index++)
            {
                yield return this[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
