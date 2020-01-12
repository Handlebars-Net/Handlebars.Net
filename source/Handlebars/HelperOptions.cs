using System;
using System.Collections.Generic;
using System.IO;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet
{
    public sealed class HelperOptions
    {
        internal HelperOptions(
            Action<TextWriter, object> template,
            Action<TextWriter, object> inverse,
            BlockParamsValueProvider blockParamsValueProvider)
        {
            Template = template;
            Inverse = inverse;
            BlockParams = blockParamsValueProvider.Configure;
        }

        public Action<TextWriter, object> Template { get; }

        public Action<TextWriter, object> Inverse { get; }

        public Action<ConfigureBlockParams> BlockParams { get; }
    }
}

