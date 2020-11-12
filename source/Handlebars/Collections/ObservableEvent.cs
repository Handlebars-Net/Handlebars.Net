namespace HandlebarsDotNet.Collections
{
    public abstract class ObservableEvent<T>
    {
        public T Value { get; }

        public ObservableEvent(T value) => Value = value;
    }
}