using System;

namespace HandlebarsDotNet
{
    internal class DisposableContainer<T> : IDisposable
    {
        private readonly Action<T> _onDispose;
        public T Value { get; }

        public DisposableContainer(T value, Action<T> onDispose)
        {
            _onDispose = onDispose;
            Value = value;
        }
        
        public void Dispose()
        {
            _onDispose(Value);
        }
    }
}