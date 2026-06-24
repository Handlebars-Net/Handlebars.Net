namespace HandlebarsDotNet.Collections
{
    public abstract class ObservableEvent<T> : IObservableEvent<T>
    {
        public T Value { get; }

        protected ObservableEvent(T value) => Value = value;
    }
}