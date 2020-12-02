using System;
using System.Runtime.CompilerServices;
using HandlebarsDotNet.Pools;

namespace HandlebarsDotNet.Collections
{
    internal readonly struct ImmutableStack<T>
    {
        private readonly Node _container;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImmutableStack(T value, Node parent)
            :this(Node.Create(value, parent))
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
            _container.Dispose();
            return new ImmutableStack<T>(_container.Parent);
        }
        
        private sealed class Node : IDisposable
        {
            private static readonly InternalObjectPool<Node, Policy> Pool = new InternalObjectPool<Node, Policy>(new Policy());
            
            public Node Parent;
            public T Value;

            public static Node Create(T value = default, Node parent = null)
            {
                var item = Pool.Get();
                item.Value = value;
                item.Parent = parent;
                return item;
            }
            
            private Node() { }
            
            private struct Policy : IInternalObjectPoolPolicy<Node>
            {
                public Node Create() => new Node();

                public bool Return(Node item)
                {
                    item.Parent = null;
                    item.Value = default;
                    return true;
                }
            }

            public void Dispose() => Pool.Return(this);
        }
    }
}