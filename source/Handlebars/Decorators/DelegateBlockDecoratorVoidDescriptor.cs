using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Decorators
{
    public sealed class DelegateBlockDecoratorVoidDescriptor : IDecoratorDescriptor<BlockDecoratorOptions>
    {
        private readonly HandlebarsBlockDecoratorVoid _helper;

        public DelegateBlockDecoratorVoidDescriptor(string name, HandlebarsBlockDecoratorVoid helper)
        {
            _helper = helper;
            Name = name;
        }

        public TemplateDelegate Invoke(in TemplateDelegate function, in BlockDecoratorOptions options, in Context context, in Arguments arguments)
        {
            _helper(function, options, context, arguments);
            return function;
        }

        public PathInfo Name { get; }
    }
}