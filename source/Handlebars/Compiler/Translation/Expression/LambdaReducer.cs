using System;
using System.IO;

namespace HandlebarsDotNet.Compiler
{
    internal struct LambdaReducer
    {
        private readonly BindingContext _context;
        private readonly Action<BindingContext, TextWriter, object> _direct;
        private readonly Action<BindingContext, TextWriter, object> _inverse;

        public LambdaReducer(BindingContext context, Action<BindingContext, TextWriter, object> direct, Action<BindingContext, TextWriter, object> inverse) : this()
        {
            _context = context;
            _direct = direct;
            _inverse = inverse;

            Direct = DirectCall;
            Inverse = InverseCall;
        }

        public Action<TextWriter, object> Direct { get; }
        public Action<TextWriter, object> Inverse { get; }

        private void DirectCall(TextWriter writer, object arg)
        {
            _direct(_context, writer, arg);
        }
            
        private void InverseCall(TextWriter writer, object arg)
        {
            _inverse(_context, writer, arg);
        }
    }
}