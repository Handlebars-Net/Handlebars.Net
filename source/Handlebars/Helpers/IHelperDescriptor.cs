using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.Helpers
{
    public interface IHelperDescriptor
    {
        PathInfo Name { get; }
    }
    
    public interface IHelperDescriptor<TOptions> : IHelperDescriptor
        where TOptions: struct, IHelperOptions
    {
        object Invoke(in TOptions options, in Context context, in Arguments arguments);
        void Invoke(in EncodedTextWriter output, in TOptions options, in Context context, in Arguments arguments);
    }
}