using System;

namespace HandlebarsDotNet
{
    /// <summary>
    /// General <c>Handlebars</c> exception
    /// </summary>
    public class HandlebarsException : Exception
    {
        private string? _sourceHint;

        public HandlebarsException(string message)
            : this(message, null, null)
        {
        }

        internal HandlebarsException(string message, IReaderContext? context = null)
            : this(message, null, context)
        {
        }

        public HandlebarsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        internal HandlebarsException(string message, Exception? innerException, IReaderContext? context = null)
            : base(FormatMessage(message, context), innerException)
        {
            if (context == null) return;

            Line = context.LineNumber;
            Column = context.CharNumber;
        }

        /// <summary>
        /// Zero-based line in the template source where the error was detected,
        /// or <c>null</c> when the location is not known.
        /// Matches the line reported in the "Occured at" part of <see cref="Message"/>.
        /// </summary>
        public int? Line { get; }

        /// <summary>
        /// Zero-based column in the template source where the error was detected,
        /// or <c>null</c> when the location is not known.
        /// Matches the column reported in the "Occured at" part of <see cref="Message"/>.
        /// </summary>
        public int? Column { get; }

        /// <inheritdoc />
        /// <remarks>
        /// When the location of a compile-time error is known, the message ends with the
        /// template source line containing the error and a caret (<c>^</c>) under <see cref="Column"/>.
        /// </remarks>
        public override string Message => _sourceHint == null ? base.Message : base.Message + _sourceHint;

        internal bool HasSourceHint => _sourceHint != null;

        internal void SetSourceHint(string sourceHint)
        {
            _sourceHint = sourceHint;
        }

        private static string FormatMessage(string message, IReaderContext? context)
        {
            if (context == null) return message;

            return $"{message}\n\nOccured at: {context.LineNumber}:{context.CharNumber}";
        }
    }
}
