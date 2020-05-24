using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;

namespace HandlebarsDotNet.ValueProviders
{
    internal class BindingContextValueProvider : IValueProvider
    {
        private readonly BindingContext _context;

        public BindingContextValueProvider(BindingContext context)
        {
            _context = context;
        }

        public ValueTypes SupportedValueTypes { get; } = ValueTypes.Context;

        public bool TryGetValue(ref ChainSegment segment, out object value)
        {
            switch (segment.LowerInvariant)
            {
                case "root":
                    value = _context.Root;
                    return true;
                
                case "parent":
                    value = _context.ParentContext;
                    return true;
                
                default:
                    return TryGetContextVariable(_context.Value, ref segment, out value);
            }
        }
        
        private bool TryGetContextVariable(object instance, ref ChainSegment segment, out object value)
        {
            value = null;
            if (instance == null) return false;

            var instanceType = instance.GetType();
            var descriptorProvider = _context.Configuration.ObjectDescriptorProvider;
            if(
                descriptorProvider.CanHandleType(instanceType) && 
                descriptorProvider.TryGetDescriptor(instanceType, out var descriptor) &&
                descriptor.MemberAccessor.TryGetValue(instance, instanceType, segment.Value, out value)
            )
            {
                return true;
            }

            return _context.ParentContext?.TryGetContextVariable(ref segment, out value) ?? false;
        }

        public void Dispose()
        {
        }
    }
}