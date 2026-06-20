using System.Runtime.CompilerServices;
using HandlebarsDotNet.IO;

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
        )  where THelperDescriptor: class, IHelperDescriptor<TOptions>
            where TOptions : struct, IHelperOptions
        {
            var configuration = options.Frame.Configuration;
            using var writer = ReusableStringWriter.Get(configuration.FormatProvider);
            using var output = new EncodedTextWriter(writer, configuration.TextEncoder, FormatterProvider.Current, configuration.NoEscape);

            descriptor.Invoke(output, options, context, arguments);

            // Return a SafeString so the captured output — which already has the correct
            // encoding applied by the EncodedTextWriter — is not encoded a second time
            // when it is passed as an argument to an outer helper.
            return new SafeString(output.ToString());
        }
    }
}