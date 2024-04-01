using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Decorators
{
    public sealed class EmptyBlockDecorator : IDecoratorDescriptor<BlockDecoratorOptions>
    {
        public EmptyBlockDecorator(PathInfo name) => Name = name;

        public TemplateDelegate Invoke(in TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments)
        {
            return function;
        }

        public PathInfo Name { get; }
    }
}