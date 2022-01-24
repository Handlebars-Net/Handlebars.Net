using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Decorators
{
    public sealed class EmptyDecorator : IDecoratorDescriptor<DecoratorOptions>
    {
        public EmptyDecorator(PathInfo name) => Name = name;

        public TemplateDelegate Invoke(in TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments) => function;

        public PathInfo Name { get; }
    }
}