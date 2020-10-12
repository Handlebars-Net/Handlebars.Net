using System;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.ValueProviders
{
    [Flags]
    internal enum ValueTypes
    {
        Context = 1,
        All = 2
    }
    
    internal interface IValueProvider : IDisposable
    {
        ValueTypes SupportedValueTypes { get; }
        bool TryGetValue(ref ChainSegment segment, out object value);
    }
}