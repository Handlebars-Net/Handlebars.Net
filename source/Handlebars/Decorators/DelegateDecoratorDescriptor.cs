using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Decorators
{
    public sealed class DelegateDecoratorDescriptor : IDecoratorDescriptor<DecoratorOptions>
    {
        private readonly HandlebarsDecorator _helper;

        public DelegateDecoratorDescriptor(string name, HandlebarsDecorator helper)
        {
            _helper = helper;
            Name = name;
        }

        public TemplateDelegate Invoke(in TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments)
        {
            return _helper(function, options, context, arguments);
        }

        public PathInfo Name { get; }
    }
}