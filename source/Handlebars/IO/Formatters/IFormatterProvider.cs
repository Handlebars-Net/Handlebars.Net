using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO
{
    public interface IFormatterProvider
    {
        bool TryCreateFormatter(Type type, [NotNullWhen(true)] out IFormatter? formatter);
    }
}