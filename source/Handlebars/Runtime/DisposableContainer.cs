using System;
using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Runtime
{
    public readonly struct DisposableContainer<T, TState> : IDisposable
    {
        private readonly TState _state;
        private readonly Action<T, TState> _onDispose;
        public readonly T Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DisposableContainer(T value, TState state, Action<T, TState> onDispose)
        {
            _onDispose = onDispose;
            Value = value;
            _state = state;
        }
        
        public void Dispose() => _onDispose(Value, _state);
    }
    
    public readonly struct DisposableContainer<T> : IDisposable
    {
        private readonly Action<T> _onDispose;
        public readonly T Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DisposableContainer(T value, Action<T> onDispose)
        {
            _onDispose = onDispose;
            Value = value;
        }
        
        public void Dispose() => _onDispose(Value);
    }
    
    public readonly struct DisposableContainer : IDisposable
    {
        private readonly Action _onDispose;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DisposableContainer(Action onDispose) => _onDispose = onDispose;

        public void Dispose() => _onDispose();
    }
}