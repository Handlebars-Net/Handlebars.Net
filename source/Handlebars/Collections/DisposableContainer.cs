using System;

namespace HandlebarsDotNet
{
    internal readonly struct DisposableContainer<T> : IDisposable
    {
        private readonly Action<T> _onDispose;
        public readonly T Value;

        public DisposableContainer(T value, Action<T> onDispose)
        {
            _onDispose = onDispose;
            Value = value;
        }
        
        public void Dispose() => _onDispose(Value);
    }
}