using System;
using System.Text;

namespace HandlebarsDotNet
{
    internal class StringBuilderPool : InternalObjectPool<StringBuilder>
    {
        private static readonly Lazy<StringBuilderPool> Lazy = new Lazy<StringBuilderPool>(() => new StringBuilderPool());
        
        public static StringBuilderPool Shared => Lazy.Value;

        public StringBuilderPool(int initialCapacity = 16) 
            : base(new StringBuilderPooledObjectPolicy{ InitialCapacity = initialCapacity })
        {
        }
        
        public class StringBuilderPooledObjectPolicy : IInternalObjectPoolPolicy<StringBuilder>
        {
            public int InitialCapacity { get; set; } = 100;

            public int MaximumRetainedCapacity { get; set; } = 4096;

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