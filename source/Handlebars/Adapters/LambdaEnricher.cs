using System;
using System.IO;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet.Adapters
{
    internal class LambdaEnricher
    {
        private readonly Action<TextWriter, object> _direct;
        private readonly Action<TextWriter, object> _inverse;

        public LambdaEnricher(Action<TextWriter, object> direct, Action<TextWriter, object> inverse)
        {
            _direct = direct;
            _inverse = inverse;

            Direct = (context, writer, arg) => _direct(writer, arg);
            Inverse = (context, writer, arg) => _inverse(writer, arg);
        }

        public readonly Action<BindingContext, TextWriter, object> Direct;
        public readonly Action<BindingContext, TextWriter, object> Inverse;
    }
}