using System;
using System.Collections;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Polyfills;

namespace HandlebarsDotNet.Compiler
{
    internal static class DeferredSectionBlockHelper
    {
        private static readonly ChainSegment[] BlockParamsVariables = ArrayEx.Empty<ChainSegment>();

        public static void PlainHelper(
            BindingContext context, 
            EncodedTextWriter writer,
            object value,
            TemplateDelegate body, 
            TemplateDelegate inverse
        )
        {
            RenderSection(value, context, writer, body, inverse);
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

