using System.Runtime.CompilerServices;

namespace HandlebarsDotNet.Helpers
{
    public static class HelperExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReturnInvoke<THelperDescriptor, TOptions>(
            this THelperDescriptor descriptor, 
            in TOptions options, 
            in Context context, 
            in Arguments arguments
        )  where THelperDescriptor: IHelperDescriptor<TOptions> 
            where TOptions : struct, IHelperOptions
        {
            var configuration = options.Frame.Configuration;
            using var writer = ReusableStringWriter.Get(configuration.FormatProvider);
            using var output = new EncodedTextWriter(writer, configuration.TextEncoder, configuration.UnresolvedBindingFormatter, configuration.NoEscape);

            descriptor.Invoke(output, options, context, arguments);

            return output.ToString();
        }
    }
}