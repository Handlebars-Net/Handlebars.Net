using System.Runtime.CompilerServices;
using HandlebarsDotNet.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Decorators;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.IO;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet
{
    /// <summary>
    /// Contains properties accessible withing <see cref="HandlebarsBlockDecorator"/> function 
    /// </summary>
    public readonly struct BlockDecoratorOptions : IDecoratorOptions
    {
        internal readonly TemplateDelegate OriginalTemplate;

        public BindingContext Frame { get; }
        
        public readonly ChainSegment[] BlockVariables;

        internal BlockDecoratorOptions(
            PathInfo name,
            TemplateDelegate template,
            ChainSegment[] blockParamsValues,
            BindingContext frame)
        {
            Name = name;
            OriginalTemplate = template;
            Frame = frame;
            BlockVariables = blockParamsValues;
        }
        
        public DataValues Data => new DataValues(Frame);
        
        public PathInfo Name { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingContext CreateFrame(object value = null) => Frame.CreateFrame(value);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BindingContext CreateFrame(Context value) => Frame.CreateFrame(value.Value);
        
        /// <summary>
        /// BlockHelper body
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Template()
        {
            using var writer = ReusableStringWriter.Get();
            using var encodedTextWriter = new EncodedTextWriter(writer, Frame.Configuration.TextEncoder, FormatterProvider.Current);
            
            OriginalTemplate(encodedTextWriter, Frame);

            return encodedTextWriter.ToString();
        }

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
        
        IIndexed<string, IHelperDescriptor<HelperOptions>> IHelpersRegistry.GetHelpers() => Frame.Helpers;

        IIndexed<string, IHelperDescriptor<BlockHelperOptions>> IHelpersRegistry.GetBlockHelpers() => Frame.BlockHelpers;
    }
}