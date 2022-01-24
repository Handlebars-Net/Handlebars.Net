using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;

namespace HandlebarsDotNet.Decorators
{
    public sealed class DelegateDecoratorVoidDescriptor : IDecoratorDescriptor<DecoratorOptions>
    {
        private readonly HandlebarsDecoratorVoid _helper;

        public DelegateDecoratorVoidDescriptor(string name, HandlebarsDecoratorVoid helper)
        {
            _helper = helper;
            Name = name;
        }

        public TemplateDelegate Invoke(in TemplateDelegate function, in DecoratorOptions options, in Context context, in Arguments arguments)
        {
            _helper(function, options, context, arguments);
            return function;
        }

        public PathInfo Name { get; }
    }
}