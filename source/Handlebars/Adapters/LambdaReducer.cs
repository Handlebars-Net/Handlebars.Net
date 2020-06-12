using System;
using System.IO;
using HandlebarsDotNet.Compiler;

namespace HandlebarsDotNet.Adapters
{
    internal class LambdaReducer
    {
        private readonly BindingContext _context;
        private readonly Action<BindingContext, TextWriter, object> _direct;
        private readonly Action<BindingContext, TextWriter, object> _inverse;

        public LambdaReducer(BindingContext context, Action<BindingContext, TextWriter, object> direct, Action<BindingContext, TextWriter, object> inverse)
        {
            _context = context;
            _direct = direct;
            _inverse = inverse;

            Direct = (writer, arg) => _direct(_context, writer, arg);
            Inverse = (writer, arg) => _inverse(_context, writer, arg);
        }

        public readonly Action<TextWriter, object> Direct;
        public readonly Action<TextWriter, object> Inverse;
    }
}