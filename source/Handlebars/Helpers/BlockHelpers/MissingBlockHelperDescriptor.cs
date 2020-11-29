using System.Collections;
using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Helpers.BlockHelpers
{
    public sealed class MissingBlockHelperDescriptor : IHelperDescriptor<BlockHelperOptions>
    {
        private static readonly ChainSegment[] BlockParamsVariables = ArrayEx.Empty<ChainSegment>();
    
        public PathInfo Name { get; } = "missingBlockHelper";

        public object Invoke(in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            return this.ReturnInvoke(options, context, arguments);
        }

        public void Invoke(in EncodedTextWriter output, in BlockHelperOptions options, in Context context, in Arguments arguments)
        {
            if(arguments.Length > 0) throw new HandlebarsRuntimeException($"Template references a helper that cannot be resolved. BlockHelper '{options.Name}'");
            
            var bindingContext = options.Frame;
            var value = PathResolver.ResolvePath(bindingContext, options.Name);
            RenderSection(value, bindingContext, output, options.OriginalTemplate, options.OriginalInverse);
        }

        private static void RenderSection(object value,
            BindingContext context,
            EncodedTextWriter writer,
            TemplateDelegate body,
            TemplateDelegate inversion)
        {
            switch (value)
            {
                case bool boolValue when boolValue:
                    body(writer, context);
                    return;
                
                case null:
                case object _ when HandlebarsUtils.IsFalsyOrEmpty(value):
                    inversion(writer, context);
                    return;

                case string _:
                {
                    using var frame = context.CreateFrame(value);
                    body(writer, frame);
                    return;
                }
                
                case IEnumerable enumerable:
                    Iterator.Iterate(context, writer, BlockParamsVariables, enumerable, body, inversion);
                    break;
                
                default:
                {
                    using var frame = context.CreateFrame(value);
                    body(writer, frame);
                    break;
                }
            }
        }
    }
}