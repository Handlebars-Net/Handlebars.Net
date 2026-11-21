using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HandlebarsDotNet
{
    internal sealed class ExtendedStringReader : TextReader
    {
        // Upper bound on how far ahead the reader looks to complete a source line for an error hint.
        private const int MaxLookAhead = 4096;

        private int _linePos;
        private int _charPos;
        private int _matched;

        // Everything read so far, kept so compile errors can show the offending source line.
        private readonly StringBuilder _consumed = new StringBuilder();
        // Offset in _consumed at which each line (as counted by _linePos) starts.
        private readonly List<int> _lineStarts = new List<int> { 0 };

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
            if (c >= 0)
            {
                _consumed.Append((char) c);
                AdvancePosition((char) c);
            }
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
                _lineStarts.Add(_consumed.Length);

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

        /// <summary>
        /// Returns the text of the given line, as numbered by <see cref="GetContext"/>, without its line terminator.
        /// If the line has not been fully read yet, reads ahead (a bounded amount) to complete it.
        /// Returns <c>null</c> when the line is not available.
        /// </summary>
        public string? GetLine(int lineNumber)
        {
            if (lineNumber < 0) return null;

            ReadAheadToEndOfLine(lineNumber);

            if (lineNumber >= _lineStarts.Count) return null;

            var start = _lineStarts[lineNumber];
            var end = lineNumber + 1 < _lineStarts.Count
                ? _lineStarts[lineNumber + 1] - Environment.NewLine.Length
                : _consumed.Length;

            return _consumed.ToString(start, Math.Max(0, end - start));
        }

        private void ReadAheadToEndOfLine(int lineNumber)
        {
            try
            {
                // keep reading until the requested line is terminated or the source ends
                var budget = MaxLookAhead;
                while (lineNumber + 1 >= _lineStarts.Count && budget > 0 && Read() != -1)
                {
                    budget--;
                }
            }
            catch (ObjectDisposedException)
            {
                // the underlying reader is already closed: use what was read
            }
        }

        private class ReaderContext : IReaderContext
        {
            public int LineNumber { get; set; }

            public int CharNumber { get; set; }
        }
    }
}
