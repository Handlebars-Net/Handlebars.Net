using System;
using System.Collections.Generic;

namespace HandlebarsDotNet.Collections
{
    internal class ObserverBuilder<T>
    {
        private readonly Dictionary<Type, List<Action<T>>> _handlers = new Dictionary<Type, List<Action<T>>>();
        
        public ObserverBuilder<T> OnEvent<TEvent>(Action<TEvent> handler, Func<TEvent, bool> predicate = null) where TEvent: T
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                handlers = new List<Action<T>>();
                _handlers.Add(typeof(TEvent), handlers);
            }
            
            handlers.Add(@event =>
            {
                if(predicate?.Invoke((TEvent) @event) ?? true) handler((TEvent) @event);
            });

            return this;
        }

        public IObserver<T> Build() => new Observer(_handlers);

        private class Observer : IObserver<T>
        {
            private readonly Dictionary<Type, List<Action<T>>> _handlers;

            public Observer(Dictionary<Type, List<Action<T>>> handlers) => _handlers = handlers;

            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(T value)
            {
                if (!_handlers.TryGetValue(value.GetType(), out var handlers)) return;
                
                for (int index = 0; index < handlers.Count; index++)
                {
                    handlers[index](value);
                }
            }
        }
    }
}