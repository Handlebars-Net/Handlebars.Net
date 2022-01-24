using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Decorators
{
    public sealed class DelegateBlockDecoratorDescriptor : IDecoratorDescriptor<BlockDecoratorOptions>
    {
        private readonly HandlebarsBlockDecorator _helper;

        public DelegateBlockDecoratorDescriptor(string name, HandlebarsBlockDecorator helper)
        {
            _helper = helper;
            Name = name;
        }

        public TemplateDelegate Invoke(in TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments)
        {
            return _helper(function, options, context, arguments);
        }

        public PathInfo Name { get; }
    }
}