using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    public interface IHelperOptions
    {
        BindingContext Frame { get; }
        DataValues Data { get; }
        PathInfo Name { get; }
    }
}