using System.Runtime.CompilerServices;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Contains properties accessible withing <see cref="HandlebarsBlockHelper"/> function 
    /// </summary>
    public readonly struct BlockHelperOptions : IHelperOptions
    {
        internal readonly TemplateDelegate OriginalTemplate;
        internal readonly TemplateDelegate OriginalInverse;
        
        public BindingContext Frame { get; }
        
        public readonly ChainSegment[] BlockVariables;

        internal BlockHelperOptions(
            PathInfo name,
            TemplateDelegate template,
            TemplateDelegate inverse,
            ChainSegment[] blockParamsValues,
            BindingContext frame
        )
        {
            Name = name;
            OriginalTemplate = template;
            OriginalInverse = inverse;
            Frame = frame;
            BlockVariables = blockParamsValues;
        }
        
        public DataValues Data => new DataValues(Frame);
        
        public PathInfo Name { get; }

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
    }
}

