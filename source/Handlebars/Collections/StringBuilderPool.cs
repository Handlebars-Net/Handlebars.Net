using System;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace HandlebarsDotNet
{
    internal class StringBuilderPool : DefaultObjectPool<StringBuilder>
    {
        private static readonly Lazy<StringBuilderPool> Lazy = new Lazy<StringBuilderPool>(() => new StringBuilderPool());
        
        public static StringBuilderPool Shared => Lazy.Value;

        public StringBuilderPool(int initialCapacity = 16) 
            : base(new StringBuilderPooledObjectPolicy{ InitialCapacity = initialCapacity })
        {
        }
    }
}