using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Runtime
{
    public class Ref<T> where T: class
    {
        private readonly Ref<T> _parent;
        private T _value;

        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value ?? _parent?.Value;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _value = value;
        }

        public Ref() { }
        
        public Ref(T value) => _value = value;

        public Ref(Ref<T> parent) => _parent = parent;

        public Ref(T value, Ref<T> parent)
        {
            _value = value;
            _parent = parent;
        }
    }
}