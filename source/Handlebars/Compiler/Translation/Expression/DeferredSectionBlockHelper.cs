using System;
using System.Collections;
using System.IO;

namespace HandlebarsDotNet.Compiler
{
    internal static class DeferredSectionBlockHelper
    {
        public static void Helper(BindingContext context, char prefix, object value,
            Action<BindingContext, TextWriter, object> body, Action<BindingContext, TextWriter, object> inverse,
            BlockParamsValueProvider blockParamsValueProvider)
        {
            if (prefix == '#')
            {
                RenderSection(value, context, body, inverse, blockParamsValueProvider);
            }
            else
            {
                RenderSection(value, context, inverse, body, blockParamsValueProvider);
            }
        }
        
        private static void RenderSection(
            object value, 
            BindingContext context,
            Action<BindingContext, TextWriter, object> body, 
            Action<BindingContext, TextWriter, object> inversion,
            BlockParamsValueProvider blockParamsValueProvider
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
                    Iterator.Iterate(context, blockParamsValueProvider, enumerable, body, inversion);
                    break;
                
                default:
                    body(context, context.TextWriter, value);
                    break;
            }
        }
    }
}

