using System;

namespace HandlebarsDotNet.IO
{
    public interface IFormatterProvider
    {
        bool TryCreateFormatter(Type type, out IFormatter formatter);
    }
}