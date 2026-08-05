namespace HandlebarsDotNet.Collections;

public interface IObservableEvent<out T>
{
    T Value { get; }
}