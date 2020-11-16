using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    public interface IHelperOptions
    {
        BindingContext Frame { get; }
        DataValues Data { get; }
    }

    /// <summary>
    /// Contains properties accessible withing <see cref="HandlebarsBlockHelper"/> function 
    /// </summary>
    public readonly struct BlockHelperOptions : IHelperOptions
    {
        private readonly IIndexed<string, object> _extensions;
        
        internal readonly TemplateDelegate OriginalTemplate;
        internal readonly TemplateDelegate OriginalInverse;
        
        public BindingContext Frame { get; }
        
        public readonly ChainSegment[] BlockVariables;

        internal BlockHelperOptions(
            TemplateDelegate template,
            TemplateDelegate inverse,
            ChainSegment[] blockParamsValues,
            BindingContext frame
        )
        {
            _extensions = frame.Bag;
            
            OriginalTemplate = template;
            OriginalInverse = inverse;
            Frame = frame;
            BlockVariables = blockParamsValues;
        }
        
        public DataValues Data => new DataValues(Frame);
        
        public BlockParamsValues BlockParams => new BlockParamsValues(Frame, BlockVariables);
        
        /// <summary>
        /// BlockHelper body
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Template(in EncodedTextWriter writer, object context)
        {
            if (context is BindingContext bindingContext)
            {
                OriginalTemplate(writer, bindingContext);
                return;
            }
                
            using var frame = Frame.CreateFrame(context);
            OriginalTemplate(writer, frame);
        }
        
        /// <summary>
        /// BlockHelper body
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Template(in EncodedTextWriter writer, in Context context) => Template(writer, context.Value);
        
        /// <summary>
        /// BlockHelper body
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Template(in EncodedTextWriter writer, BindingContext context) => OriginalTemplate(writer, context);

        /// <summary>
        /// BlockHelper body
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inverse(in EncodedTextWriter writer, object context)
        {
            if (context is BindingContext bindingContext)
            {
                OriginalInverse(writer, bindingContext);
                return;
            }
                
            using var frame = Frame.CreateFrame(context);
            OriginalInverse(writer, frame);
        }

        /// <summary>
        /// BlockHelper body
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inverse(in EncodedTextWriter writer, in Context context) => Inverse(writer, context.Value);
        
        /// <summary>
        /// BlockHelper body
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inverse(in EncodedTextWriter writer, BindingContext context) => OriginalInverse(writer, context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingContext CreateFrame(object value = null) => Frame.CreateFrame(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingContext CreateFrame(Context value) => Frame.CreateFrame(value.Value);

        /// <summary>
        /// Provides access to dynamic options
        /// </summary>
        /// <param name="property"></param>
        public object this[string property]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _extensions.TryGetValue(property, out var value) ? value : null;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal set => _extensions.AddOrReplace(property, value);
        }

        /// <summary>
        /// Provides access to dynamic data entries in a typed manner
        /// </summary>
        /// <param name="property"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetValue<T>(string property) => (T) this[property];
    }
}

