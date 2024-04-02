#if NET451
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

namespace HandlebarsDotNet.Polyfills
{
    public sealed class AsyncLocal<T>
    {
        private const string Slot = "__AsyncLocalSlot";

        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => CallContext.LogicalGetData(Slot) is Container container
                ? container.Value
                : default;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => CallContext.LogicalSetData(Slot, new Container(value));
        }

        [Serializable]
        private class Container
        {
            public readonly T Value;

            public Container(T value)
            {
                Value = value;
            }
        }
    }
}
#endif