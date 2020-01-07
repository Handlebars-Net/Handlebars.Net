namespace HandlebarsDotNet.Compiler
{
    internal interface IValueProvider
    {
        bool ProvidesNonContextVariables { get; }
        bool TryGetValue(string memberName, out object value);
    }
}