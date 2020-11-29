using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Collections
{
    internal readonly struct ImmutableStack<T>
    {
        private readonly Node _container;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableStack(T value, Node parent)
            :this(new Node { Value = value, Parent = parent })
        {
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableStack(Node container) => _container = container;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableStack<T> Push(T value) => new ImmutableStack<T>(value, _container);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            return _container == null 
                ? default 
                : _container.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableStack<T> Pop(out T value)
        {
            if (_container == null)
            {
                value = default;
                return this;
            }
            
            value = _container.Value;
            return new ImmutableStack<T>(_container.Parent);
        }
        
        private class Node
        {
            public Node Parent;
            public T Value;
        }
    }
}