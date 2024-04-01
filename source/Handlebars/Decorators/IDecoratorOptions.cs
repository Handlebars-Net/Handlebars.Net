using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Decorators
{
    public interface IDecoratorOptions : IOptions, IHelpersRegistry
    {
        DataValues Data { get; }
        PathInfo Name { get; }
    }
}