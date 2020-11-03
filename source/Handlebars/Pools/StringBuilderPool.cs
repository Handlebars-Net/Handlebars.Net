using System;
using System.Text;

namespace HandlebarsDotNet.Pools
{
    internal class StringBuilderPool : InternalObjectPool<StringBuilder, StringBuilderPool.StringBuilderPooledObjectPolicy>
    {
        private static readonly Lazy<StringBuilderPool> Lazy = new Lazy<StringBuilderPool>(() => new StringBuilderPool());
        
        public static StringBuilderPool Shared => Lazy.Value;

        public StringBuilderPool(int initialCapacity = 16) 
            : base(new StringBuilderPooledObjectPolicy(initialCapacity))
        {
        }
        
        public readonly struct StringBuilderPooledObjectPolicy : IInternalObjectPoolPolicy<StringBuilder>
        {
            public StringBuilderPooledObjectPolicy(int initialCapacity, int maximumRetainedCapacity = 4096)
            {
                InitialCapacity = initialCapacity;
                MaximumRetainedCapacity = maximumRetainedCapacity;
            }

            public readonly int InitialCapacity;

            public readonly int MaximumRetainedCapacity;

            public StringBuilder Create()
            {
                return new StringBuilder(InitialCapacity);
            }

            public bool Return(StringBuilder obj)
            {
                if (obj.Capacity > MaximumRetainedCapacity)
                    return false;
                
                obj.Clear();
                return true;
            }
        }
    }
}