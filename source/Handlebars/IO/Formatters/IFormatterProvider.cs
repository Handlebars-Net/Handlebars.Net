using System;
using System.Diagnostics.CodeAnalysis;

namespace HandlebarsDotNet.IO
{
    public interface IFormatterProvider
    {
        bool TryCreateFormatter(Type type, [MaybeNullWhen(false)] out IFormatter formatter);
    }
}