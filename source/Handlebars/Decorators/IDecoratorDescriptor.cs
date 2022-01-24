using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Decorators
{
    public interface IDecoratorDescriptor
    {
        PathInfo Name { get; }
    }
    
    public interface IDecoratorDescriptor<TOptions> : IDecoratorDescriptor, IDescriptor<TOptions>
        where TOptions: struct, IDecoratorOptions
    {
        TemplateDelegate Invoke(in TemplateDelegate function, in TOptions options, in Context context, in Arguments arguments);
    }
}