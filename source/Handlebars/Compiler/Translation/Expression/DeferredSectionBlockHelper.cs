using System;
using System.Collections;
using System.IO;
using HandlebarsDotNet.Compiler.Structure.Path;
using HandlebarsDotNet.Polyfills;
using HandlebarsDotNet.ValueProviders;

namespace HandlebarsDotNet.Compiler
{
    internal static class DeferredSectionBlockHelper
    {
        private static readonly ChainSegment[] BlockParamsVariables = ArrayEx.Empty<ChainSegment>();

        public static void PlainHelper(BindingContext context, object value,
            Action<BindingContext, TextWriter, object> body, Action<BindingContext, TextWriter, object> inverse)
        {
            RenderSection(value, context, body, inverse);
        }
        
        private static void RenderSection(
            object value, 
            BindingContext context,
            Action<BindingContext, TextWriter, object> body, 
            Action<BindingContext, TextWriter, object> inversion
        )
        {
            switch (value)
            {
                case bool boolValue when boolValue:
                    body(context, context.TextWriter, context);
                    return;
                
                case null:
                case object _ when HandlebarsUtils.IsFalsyOrEmpty(value):
                    inversion(context, context.TextWriter, context);
                    return;

                case string _:
                    body(context, context.TextWriter, value);
                    return;
                
                case IEnumerable enumerable:
                    Iterator.Iterate(context, BlockParamsVariables, enumerable, body, inversion);
                    break;
                
                default:
                    body(context, context.TextWriter, value);
                    break;
            }
        }
    }
}

