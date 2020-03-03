using System;

namespace HandlebarsDotNet.Compiler
{
    [Flags]
    internal enum ValueTypes
    {
        Context = 1,
        All = 2
    }
    
    internal interface IValueProvider
    {
        ValueTypes SupportedValueTypes { get; }
        bool TryGetValue(string memberName, out object value);
    }
}