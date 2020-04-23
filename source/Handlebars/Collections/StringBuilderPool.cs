using System;
using System.Text;

namespace HandlebarsDotNet
{
    internal class StringBuilderPool : ObjectPool<StringBuilder>
    {
        private static readonly Lazy<StringBuilderPool> Lazy = new Lazy<StringBuilderPool>(() => new StringBuilderPool());
        
        private readonly int _initialCapacity;

        public static StringBuilderPool Shared => Lazy.Value;

        public StringBuilderPool(int initialCapacity = 16)
        {
            _initialCapacity = initialCapacity;
        }
        
        protected override StringBuilder CreateObject()
        {
            return new StringBuilder(_initialCapacity);
        }

        public override void PutObject(StringBuilder item)
        {
            item.Length = 0;
            base.PutObject(item);
        }
    }
}