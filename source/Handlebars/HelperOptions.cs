using System;
using System.IO;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void BlockParamsConfiguration(ConfigureBlockParams blockParamsConfiguration, params object[] dependencies);
    
    /// <summary>
    /// Contains properties accessible withing <see cref="HandlebarsBlockHelper"/> function 
    /// </summary>
    public sealed class HelperOptions
    {
        internal HelperOptions(
            Action<TextWriter, object> template,
            Action<TextWriter, object> inverse,
            BlockParamsValueProvider blockParamsValueProvider,
            HandlebarsConfiguration configuration)
        {
            Template = template;
            Inverse = inverse;
            Configuration = configuration;
            BlockParams = blockParamsValueProvider.Configure;
        }

        /// <summary>
        /// BlockHelper body
        /// </summary>
        public Action<TextWriter, object> Template { get; }

        /// <summary>
        /// BlockHelper <c>else</c> body
        /// </summary>
        public Action<TextWriter, object> Inverse { get; }

        /// <inheritdoc cref="ConfigureBlockParams"/>
        public BlockParamsConfiguration BlockParams { get; }
        
        /// <inheritdoc cref="HandlebarsConfiguration"/>
        internal HandlebarsConfiguration Configuration { get; }
    }
}

