using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public interface IHelperDescriptor
    {
        PathInfo Name { get; }
        
        HelperType Type { get; }
    }
}