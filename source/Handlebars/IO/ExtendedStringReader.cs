using System;
using System.IO;

namespace HandlebarsDotNet
{
    internal sealed class ExtendedStringReader : TextReader
    {
        private int _linePos;
        private int _charPos;
        private int _matched;

        public ExtendedStringReader(TextReader reader)
        {
            _inner = reader;
        }

        private readonly TextReader _inner;

        public override int Peek()
        {
            return _inner.Peek();
        }

        public override int Read()
        {
            var c = _inner.Read();
            if (c >= 0) AdvancePosition((char) c);
            return c;
        }

        private void AdvancePosition(char c)
        {
            if (Environment.NewLine[_matched] == c)
            {
                _matched++;
                if (_matched != Environment.NewLine.Length) return;
                
                _linePos++;
                _charPos = 0;
                _matched = 0;

                return;
            }

            _matched = 0;
            _charPos++;
        }

        public IReaderContext GetContext()
        {
            return new ReaderContext
            {
                LineNumber = _linePos,
                CharNumber = _charPos
            };
        }

        private class ReaderContext : IReaderContext
        {
            public int LineNumber { get; set; }

            public int CharNumber { get; set; }
        }
    }
}